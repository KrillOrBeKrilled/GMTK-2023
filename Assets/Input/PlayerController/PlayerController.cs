using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Input
{
    /// <summary>
    /// Class to handle character controls
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // --------------- Player State --------------
        private static IdleState _idle;
        private static MovingState _moving;
        private static GameOverState _gameOver;
        private IPlayerState _state;
        public UnityEvent<IPlayerState> OnPlayerStateChanged { get; private set; }

        // For movement testing, allow speeds to be set through the editor
        [Header("State speed parameters")]
        [SerializeField] private float _speed, _jumpingForce;

        private float _direction;
        
        // ------------- Trap Deployment -------------
        [SerializeField] private List<GameObject> _trapPrefabs;
        private int _currentTrapIndex = 0;
        
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
        // BRAINSTORMING: Do we want to simulate player health?

        // --------------- Bookkeeping ---------------
        private Rigidbody2D _rBody;
        private Animator _animator;

        private PlayerInputActions _playerInputActions;

        void Awake()
        {
            _rBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            _idle = new IdleState();
            _moving = new MovingState(_speed);
            _gameOver = new GameOverState();

            _state = _idle;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState>();
        }

        private void Start() {
            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            OnEnable();
        }

        private void FixedUpdate()
        {
            float directionInput = _playerInputActions.Player.Move.ReadValue<float>();
            _direction = directionInput != 0 ? directionInput : _direction;
            
            // Set animation values
            SetAnimatorValues(directionInput);
            
            // Delegate movement behaviour to state classes
            _state.Act(_rBody, _direction, EnterIdleState);

            // Check trap deployment eligibility
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

        // --------------- Getters ---------------
        public IPlayerState GetPlayerState()
        {
            return _state;
        }

        // ---------------- Input -----------------
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
            // Left out of State pattern to allow this during movement
            _rBody.AddForce(Vector2.up * _jumpingForce);
            _isGrounded = false;
            
            _animator.SetBool("is_grounded", _isGrounded);
            
            HenFlapEvent.Post(gameObject);

            // Left the ground, so trap deployment isn't possible anymore
            ClearTrapDeployment();
        }
        
        private void DeployTrap(InputAction.CallbackContext obj)
        {
            // Left out of State pattern to allow this during movement
            if(!_canDeploy || _previousTilePositions.Count < 1)
            {
                Debug.Log("Can't Deploy Trap!");
                return;
            }

            StartCoroutine(PlayBuildSoundForDuration(.3f));

            var trapToSpawn = _trapPrefabs[_currentTrapIndex];

            // Convert the origin tile position to world space
            var deploymentOrigin = _tileMap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = _direction < 0
                ? trapToSpawn.GetComponent<Traps.Trap>().GetLeftSpawnPoint(deploymentOrigin)
                : trapToSpawn.GetComponent<Traps.Trap>().GetRightSpawnPoint(deploymentOrigin);
            
            GameObject trap = Instantiate(trapToSpawn.gameObject);
            trap.transform.position = spawnPosition;
            _isColliding = true;
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
            _currentTrapIndex = 0;
            ClearTrapDeployment();
        }
        
        private void SetTrap2(InputAction.CallbackContext obj)
        {
            _currentTrapIndex = 1;
            ClearTrapDeployment();
        }

        public void EnterIdleState()
        {
            var prevState = _state;
            _state.OnExit(_idle);
            _state = _idle;
            _state.OnEnter(prevState);
        }

        public void GameOver()
        {
            
            HenDeathEvent.Post(gameObject);
            var prevState = _state;
            _state.OnExit(_gameOver);
            _state = _gameOver;
            _state.OnEnter(prevState);

            this.OnPlayerStateChanged?.Invoke(this._state);
            GameManager.Instance.OnGameOver?.Invoke();
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
        }

        private void OnDisable() {
            this._playerInputActions.Player.Move.performed -= Move;
            this._playerInputActions.Player.Move.canceled -= Idle;
            this._playerInputActions.Player.Jump.performed -= Jump;
            this._playerInputActions.Player.PlaceTrap.performed -= DeployTrap;
            this._playerInputActions.Player.SetTrap1.performed -= SetTrap1;
            this._playerInputActions.Player.SetTrap2.performed -= SetTrap2;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isGrounded = true;
                _animator.SetBool("is_grounded", _isGrounded);
            }
        }
        
        // private void OnCollisionExit2D(Collision2D collision)
        // {
        //     if (collision.gameObject.CompareTag("Ground"))
        //     {
        //         _isGrounded = false;
        //         _animator.SetBool("is_grounded", _isGrounded);
        //     }
        // }
    }
}