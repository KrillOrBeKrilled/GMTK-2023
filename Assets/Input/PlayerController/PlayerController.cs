using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Traps;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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

        private float _direction = -1;

        // ------------- Trap Deployment -------------
        // The canvas to spawn trap UI
        [SerializeField] private Canvas _trapCanvas;
        [SerializeField] private List<GameObject> _trapPrefabs;

        private int _currentTrapIndex = 0;

        public List<Trap> Traps => this._trapPrefabs.Select(prefab => prefab.GetComponent<Trap>()).ToList();
        public UnityEvent<int> OnSelectedTrapIndexChanged;

        [SerializeField] private Tilemap _trapTileMap;
        [SerializeField] private Tilemap _groundTileMap;
        [SerializeField] private GameObject _leftDeployTransform, _rightDeployTransform;

        private readonly List<Vector3Int> _previousTilePositions = new List<Vector3Int>();
        private bool _isGrounded = true, _isColliding, _canDeploy;

        // ----------------- Health ------------------
        // BRAINSTORMING: Do we want to simulate player health?
        
        // ---------------- Collider -----------------
        private ContactPoint2D _lastContact;

        // --------------- Bookkeeping ---------------
        private Rigidbody2D _rBody;
        private Animator _animator;
        private PlayerSoundsController _soundsController;

        private PlayerInputActions _playerInputActions;
        private bool _isSelectingTileSFX;

        void Awake()
        {
            _rBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _soundsController = GetComponent<PlayerSoundsController>();
            
            _idle = new IdleState();
            _moving = new MovingState(_speed);
            _gameOver = new GameOverState();

            _state = _idle;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState>();
            this.OnSelectedTrapIndexChanged = new UnityEvent<int>();
            
            _animator.SetBool("is_grounded", _isGrounded);
        }

        private void Start() {
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
            var deploymentOrigin = _trapTileMap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (_previousTilePositions.Count < 1 || deploymentOrigin != _previousTilePositions[0])
            {
                // The tile changed, so flush the tint on the previous tiles and reset the collision status
                ClearTrapDeployment();

                if (_isSelectingTileSFX)
                {
                    _soundsController.OnTileSelectMove();
                }
                else
                {
                    _isSelectingTileSFX = !_isSelectingTileSFX;
                }

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

                        validationScore = IsTileOfType<TrapTile>(_trapTileMap, tileSpacePosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        _trapTileMap.SetTileFlags(tileSpacePosition, TileFlags.None);
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
                        validationScore = IsTileOfType<TrapTile>(_trapTileMap, deploymentOrigin + prefabOffsetPosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        _trapTileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
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
            var tileWorldPosition = _trapTileMap.CellToWorld(tileSpacePosition);

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
                _trapTileMap.SetColor(previousTilePosition, new Color(1, 0, 0, 0.3f));
            }

            _canDeploy = false;
        }

        private void ValidateTrapDeployment()
        {
            // Paint all the tiles green
            foreach (Vector3Int previousTilePosition in _previousTilePositions)
            {
                _trapTileMap.SetColor(previousTilePosition, new Color(0, 1, 0, 0.3f));
            }

            _canDeploy = true;
        }

        private void ClearTrapDeployment()
        {
            foreach (Vector3Int previousTilePosition in _previousTilePositions)
            {
                _trapTileMap.SetColor(previousTilePosition, new Color(1, 1, 1, 0));
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

        public void DisablePlayerInput() {
            this._playerInputActions.Disable();
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
            
            _soundsController.OnHenJump();
            
            _animator.SetBool("is_grounded", _isGrounded);
            
            // Left the ground, so trap deployment isn't possible anymore
            ClearTrapDeployment();
            _isSelectingTileSFX = false;
        }

        private void DeployTrap(InputAction.CallbackContext obj)
        {
            // Left out of State pattern to allow this during movement
            if(!_canDeploy || _previousTilePositions.Count < 1)
            {
                Debug.Log("Can't Deploy Trap!");
                return;
            }

            var trapToSpawn = _trapPrefabs[_currentTrapIndex];
            Trap trap = trapToSpawn.GetComponent<Trap>();
            if (!CoinManager.Instance.CanAfford(trap.Cost)) {
                Debug.Log("Can't afford the trap!");
                return;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = _trapTileMap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = _direction < 0
                ? trapToSpawn.GetComponent<Trap>().GetLeftSpawnPoint(deploymentOrigin)
                : trapToSpawn.GetComponent<Trap>().GetRightSpawnPoint(deploymentOrigin);

            GameObject trapGameObject = Instantiate(trapToSpawn.gameObject);
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, _trapCanvas, _soundsController);
            _isColliding = true;
            CoinManager.Instance.ConsumeCoins(trap.Cost);
            _soundsController.OnTileSelectConfirm();
        }

        // Test functions to switch between test traps
        private void SetTrap1(InputAction.CallbackContext obj)
        {
            if (_currentTrapIndex == 0) return;
            
            _currentTrapIndex = 0;
            this.OnSelectedTrapIndexChanged?.Invoke(_currentTrapIndex);
            ClearTrapDeployment();
            
            _isSelectingTileSFX = false;
        }

        private void SetTrap2(InputAction.CallbackContext obj)
        {
            if (_currentTrapIndex == 1) return;
            
            _currentTrapIndex = 1;
            this.OnSelectedTrapIndexChanged?.Invoke(_currentTrapIndex);
            ClearTrapDeployment();
            
            _isSelectingTileSFX = false;
        }

        private void SetTrap3(InputAction.CallbackContext obj)
        {
            if (_currentTrapIndex == 2) return;
            
            _currentTrapIndex = 2;
            this.OnSelectedTrapIndexChanged?.Invoke(_currentTrapIndex);
            ClearTrapDeployment();

            _isSelectingTileSFX = false;
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
            _soundsController.OnHenDeath();
            
            var prevState = _state;
            _state.OnExit(_gameOver);
            _state = _gameOver;
            _state.OnEnter(prevState);

            this.OnPlayerStateChanged?.Invoke(this._state);
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
            if (_isGrounded) return;

            for (var i = 0; i < collision.GetContacts(collision.contacts); i++)
            {
                var contactPosition = (Vector3)collision.GetContact(i).point;
                var contactTilePosition = _groundTileMap.WorldToCell(contactPosition);

                if (!IsTileOfType<CustomGroundRuleTile>(_groundTileMap, contactTilePosition)) continue;
                
                _isGrounded = true;
                _animator.SetBool("is_grounded", _isGrounded);
                return;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            _lastContact = collision.GetContact(0);
        }

        // Called one frame after the collision, so fetch contact point from the last frame;
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_isGrounded) return;
            
            var contactPosition = (Vector3)_lastContact.point + (Vector3.down * .05f);
            var contactTilePosition = _groundTileMap.WorldToCell(contactPosition);
            
            print(IsTileOfType<CustomGroundRuleTile>(_groundTileMap, contactTilePosition));

            if (!IsTileOfType<CustomGroundRuleTile>(_groundTileMap, contactTilePosition)) return;
            
            _isGrounded = false;
            _animator.SetBool("is_grounded", _isGrounded);
            
            // Left the ground, so trap deployment isn't possible anymore
            ClearTrapDeployment();
            _isSelectingTileSFX = false;
            }
    }
}
