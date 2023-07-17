using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Player.Input.Commands;
using Traps;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Code.Scripts.Player.Input
{
    /// <summary>
    /// Class to handle character controls
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : Pawn
    {
        // --------------- Player State --------------
        private static IdleState _idle;
        private static MovingState _moving;
        private static GameOverState _gameOver;
        private IPlayerState _state;
        public UnityEvent<IPlayerState> OnPlayerStateChanged { get; private set; }
        
        // ----------------- Command -----------------
        // Stateless commands; can be copied in the list of previous commands
        private ICommand _jumpCommand;
        private ICommand _deployCommand;

        // For replay functionality, store all previous commands
        private List<ICommand> prevCommands;

        // ------------- Trap Deployment -------------
        // The canvas to spawn trap UI
        [SerializeField] private Canvas _trapCanvas;
        [SerializeField] private List<GameObject> _trapPrefabs;

        private int _currentTrapIndex;
        private float _direction = -1;

        public List<Trap> Traps => this._trapPrefabs.Select(prefab => prefab.GetComponent<Trap>()).ToList();
        public UnityEvent<int> OnSelectedTrapIndexChanged;

        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private GameObject _leftDeployTransform, _rightDeployTransform;

        private readonly List<Vector3Int> _previousTilePositions = new List<Vector3Int>();
        private bool _isGrounded = true, _isColliding, _canDeploy;

        // ------------- Sound Effects ---------------
        public AK.Wwise.Event StartBuildEvent;
        public AK.Wwise.Event StopBuildEvent;
        public AK.Wwise.Event BuildCompleteEvent;
        public AK.Wwise.Event HenDeathEvent;
        public AK.Wwise.Event HenFlapEvent;

        // ----------------- Health ------------------
        // TODO: Do we want to simulate player health?

        // --------------- Bookkeeping ---------------
        private Animator _animator;

        private PlayerInputActions _playerInputActions;

        private void Awake()
        {
            RBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            _idle = new IdleState();
            _moving = new MovingState(Speed);
            _gameOver = new GameOverState();

            _state = _idle;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState>();
            this.OnSelectedTrapIndexChanged = new UnityEvent<int>();
        }

        private void Start()
        {
            _jumpCommand = new JumpCommand(this);
            _deployCommand = new DeployCommand(this);
            prevCommands = new List<ICommand>();

            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            this.OnSelectedTrapIndexChanged?.Invoke(this._currentTrapIndex);
            OnEnable();
        }

        private void FixedUpdate()
        {
            float directionInput = _playerInputActions.Player.Move.ReadValue<float>();
            _direction = directionInput != 0 ? directionInput : _direction;

            // Set animation values
            SetAnimatorValues(directionInput);

            // Delegate movement behaviour to state classes
            _state.Act(this, _direction, prevCommands);

            // Check trap deployment eligibility
            // TODO: Does it matter which order this happens compared to the state behaviour?
            SurveyTrapDeployment();
        }

        private void SurveyTrapDeployment()
        {
            if (!_isGrounded) return;

            // Check whether to deploy left or right
            var deployChecker = _direction < 0
                ? _leftDeployTransform
                : _rightDeployTransform;
            var deployPosition = deployChecker.GetComponent<Transform>().position;
            var deploymentOrigin = _tileMap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (_previousTilePositions.Count < 1 || deploymentOrigin != _previousTilePositions[0])
            {
                // The tile changed, so flush the tint on the previous tiles and reset the collision status
                ClearTrapDeployment();

                // Get the grid placement data for the selected prefab
                var selectedTrapPrefab = _trapPrefabs[_currentTrapIndex].GetComponent<Traps.Trap>();
                var prefabPoints = _direction < 0
                        ? selectedTrapPrefab.GetLeftGridPoints()
                        : selectedTrapPrefab.GetRightGridPoints();

                // Validate the deployment of the trap with a validation score
                var validationScore = 0;
                var currentCollision = deployChecker.GetComponent<TrapOverlap>().GetCollisionData();

                if (currentCollision)
                {
                    // Simulate a broad phase of collision; if there's something in the general area, check if any of
                    // the tiles to be painted are within the collision bounds
                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        var tileSpacePosition = deploymentOrigin + prefabOffsetPosition;

                        validationScore = IsTileOfType<TrapTile>(_tileMap, tileSpacePosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        _tileMap.SetTileFlags(tileSpacePosition, TileFlags.None);
                        _previousTilePositions.Add(tileSpacePosition);

                        // Check tile collision
                        if (!_isColliding) CheckTileCollision(currentCollision, tileSpacePosition);
                    }

                    if (_isColliding)
                    {
                        InvalidateTrapDeployment();
                        return;
                    }
                }
                else
                {
                    _isColliding = false;

                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        validationScore = IsTileOfType<TrapTile>(_tileMap, deploymentOrigin + prefabOffsetPosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        _tileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                        _previousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
                    }
                }

                // If the validation score isn't high enough, paint the selected tiles an invalid color
                if (!selectedTrapPrefab.IsValidScore(validationScore))
                {
                    InvalidateTrapDeployment();
                    return;
                }

                ValidateTrapDeployment();
            }

            // Check that a trap is not already placed there
            if (_isColliding) InvalidateTrapDeployment();
        }

        private bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase
        {
            TileBase targetTile = tilemap.GetTile(position);
            if (targetTile != null && targetTile is T) return true;

            return false;
        }

        private void CheckTileCollision(Collider2D currentCollision, Vector3Int tileSpacePosition)
        {
            // Convert the origin tile position to world space
            var tileWorldPosition = _tileMap.CellToWorld(tileSpacePosition);

            // Check that the tile unit is not within the collision bounds
            var bounds = currentCollision.bounds;
            var maxBounds = bounds.max;
            var minBounds = bounds.min;

            bool vertices1 = (tileWorldPosition.x <= maxBounds.x) && (tileWorldPosition.y <= maxBounds.y);
            bool vertices2 = (tileWorldPosition.x >= minBounds.x) && (tileWorldPosition.y >= minBounds.y);

            // If any tile is found within the collider, invalidate the deployment
            if (vertices1 && vertices2)
            {
                _isColliding = true;
            }
        }

        private void InvalidateTrapDeployment()
        {
            // Paint all the tiles red
            foreach (var previousTilePosition in _previousTilePositions)
            {
                _tileMap.SetColor(previousTilePosition, new Color(1, 0, 0, 0.3f));
            }

            _canDeploy = false;
        }

        private void ValidateTrapDeployment()
        {
            // Paint all the tiles green
            foreach (Vector3Int previousTilePosition in _previousTilePositions)
            {
                _tileMap.SetColor(previousTilePosition, new Color(0, 1, 0, 0.3f));
            }

            _canDeploy = true;
        }

        private void ClearTrapDeployment()
        {
            foreach (Vector3Int previousTilePosition in _previousTilePositions)
            {
                _tileMap.SetColor(previousTilePosition, new Color(1, 1, 1, 0));
            }
            _canDeploy = false;

            // Clear the data of the previous tile
            _previousTilePositions.Clear();
        }

        // For when we put in an animator
        private void SetAnimatorValues(float inputDirection)
        {
            _animator.SetFloat("speed", Mathf.Abs(inputDirection));
            _animator.SetFloat("direction", _direction);
        }
        
        // =============== Getters ===============
        public IPlayerState GetPlayerState()
        {
            return _state;
        }

        public void DisablePlayerInput() {
            this._playerInputActions.Disable();
        }
        
        // ================ Input =================
        private void Idle(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_idle);
            _state = _idle;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }

        private void Move(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_moving);
            _state = _moving;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }

        private void Jump(InputAction.CallbackContext obj)
        {
            ExecuteCommand(_jumpCommand);
        }

        private void DeployTrap(InputAction.CallbackContext obj)
        {
            ExecuteCommand(_deployCommand);
        }

        private IEnumerator PlayBuildSoundForDuration(float durationInSeconds)
        {
            StartBuildEvent.Post(gameObject);
            yield return new WaitForSeconds(durationInSeconds);
            StopBuildEvent.Post(gameObject);
        }

        // Test functions to switch between test traps
        private void SetTrap1(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 0);
            ExecuteCommand(command);
        }

        private void SetTrap2(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 1);
            ExecuteCommand(command);
        }

        private void SetTrap3(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 2);
            ExecuteCommand(command);
        }
        
        // ======== Pawn Inherited Methods ========
        public override void Jump()
        {
            // Left out of State pattern to allow this during movement
            RBody.AddForce(Vector2.up * JumpingForce);
            _isGrounded = false;

            _animator.SetBool("is_grounded", _isGrounded);

            HenFlapEvent.Post(gameObject);

            // Left the ground, so trap deployment isn't possible anymore
            ClearTrapDeployment();
        }
        
        public override void DeployTrap()
        {
            // Left out of State pattern to allow this during movement
            if(!_canDeploy || _previousTilePositions.Count < 1)
            {
                Debug.Log("Can't Deploy Trap!");
                return;
            }

            StartCoroutine(PlayBuildSoundForDuration(.3f));

            var trapToSpawn = _trapPrefabs[_currentTrapIndex];
            Trap trap = trapToSpawn.GetComponent<Trap>();
            if (!CoinManager.Instance.CanAfford(trap.Cost)) {
                print("Can't afford the trap!");
                return;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = _tileMap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = _direction < 0
                ? trapToSpawn.GetComponent<Trap>().GetLeftSpawnPoint(deploymentOrigin)
                : trapToSpawn.GetComponent<Trap>().GetRightSpawnPoint(deploymentOrigin);

            GameObject trapGameObject = Instantiate(trapToSpawn.gameObject);
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, _trapCanvas,
                StartBuildEvent, StopBuildEvent, BuildCompleteEvent);
            _isColliding = true;
            CoinManager.Instance.ConsumeCoins(trap.Cost);
        }
        
        public override void ChangeTrap(int trapIndex)
        {
            _currentTrapIndex = trapIndex;
            this.OnSelectedTrapIndexChanged?.Invoke(trapIndex);

            ClearTrapDeployment();
        }
        
        // =============== Command ================
        // Method to execute any given command (interfaced) and record it to a list to replay
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            // Record the command to the log of previous commands
            prevCommands.Add(command);
        }

        public void GameOver()
        {
            PrintCommands();
            
            HenDeathEvent.Post(gameObject);
            var prevState = _state;
            _state.OnExit(_gameOver);
            _state = _gameOver;
            _state.OnEnter(prevState);

            this.OnPlayerStateChanged?.Invoke(this._state);
        }

        private void PrintCommands()
        {
            foreach (var command in prevCommands)
            {
                print(command);
            }
        }

        private void OnEnable() {
            // OnEnable called before Start
            // PlayerInputController.Instance and this._playerInputController may be uninitialized
            // when the scene is just started

            if (this._playerInputActions == null)
                return;

            this._playerInputActions.Player.Move.performed += Move;
            this._playerInputActions.Player.Move.canceled += Idle;
            this._playerInputActions.Player.Jump.performed += Jump;
            this._playerInputActions.Player.PlaceTrap.performed += DeployTrap;

            // Test functions to set the traps
            this._playerInputActions.Player.SetTrap1.performed += SetTrap1;
            this._playerInputActions.Player.SetTrap2.performed += SetTrap2;
            this._playerInputActions.Player.SetTrap3.performed += SetTrap3;
        }

        private void OnDisable() {
            this._playerInputActions.Player.Move.performed -= Move;
            this._playerInputActions.Player.Move.canceled -= Idle;
            this._playerInputActions.Player.Jump.performed -= Jump;
            this._playerInputActions.Player.PlaceTrap.performed -= DeployTrap;
            this._playerInputActions.Player.SetTrap1.performed -= SetTrap1;
            this._playerInputActions.Player.SetTrap2.performed -= SetTrap2;
            this._playerInputActions.Player.SetTrap3.performed -= SetTrap3;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isGrounded = true;
                _animator.SetBool("is_grounded", _isGrounded);
            }
        }
    }
}
