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
        [SerializeField] private Transform _leftDeployTransform, _rightDeployTransform;

        private readonly List<Vector3Int> _previousTilePositions = new List<Vector3Int>();
        private bool _isGrounded = true, _canDeploy;

        // ----------------- Health ------------------
        // BRAINSTORMING: Do we want to simulate player health?
        
        // --------------- Collision -----------------
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
            var deployPosition = _direction < 0
                ? _leftDeployTransform.position
                : _rightDeployTransform.position;
            var deploymentOrigin = _trapTileMap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (_previousTilePositions.Count >= 1 && deploymentOrigin == _previousTilePositions[0]) return;
            
            // The tile changed, so flush the tint on the previous tiles and reset the collision status
            ClearTrapDeployment();

            if (_isSelectingTileSFX) _soundsController.OnTileSelectMove();
            else _isSelectingTileSFX = !_isSelectingTileSFX;

            // Get the grid placement data for the selected prefab
            var selectedTrapPrefab = Traps[_currentTrapIndex];
            var prefabPoints = _direction < 0
                ? selectedTrapPrefab.GetLeftGridPoints()
                : selectedTrapPrefab.GetRightGridPoints();

            // Validate the deployment of the trap with a validation score
            var validationScore = 0;

            foreach (var prefabOffsetPosition in prefabPoints)
            {
                validationScore = IsTileOfType<TrapTile>(_trapTileMap, deploymentOrigin + prefabOffsetPosition)
                    ? ++validationScore
                    : validationScore;

                // Allow to tile to be edited
                _trapTileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                _previousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
            }

            // If the validation score isn't high enough, paint the selected tiles an invalid color
            if (!selectedTrapPrefab.IsValidScore(validationScore)) InvalidateTrapDeployment();
            else ValidateTrapDeployment();
        }

        private bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase
        {
            var targetTile = tilemap.GetTile(position);
            return targetTile is T;
        }

        private void InvalidateTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesRejectionColor(_previousTilePositions);
            _canDeploy = false;
        }
        
        private void ValidateTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesConfirmationColor(_previousTilePositions);
            _canDeploy = true;
        }
        
        private void ClearTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesBlank(_previousTilePositions);
            _canDeploy = false;
        
            // Clear the data of the previous tile
            _previousTilePositions.Clear();
        }
        
        private void SetAnimatorValues(float inputDirection)
        {
            _animator.SetFloat("speed", Mathf.Abs(inputDirection));
            _animator.SetFloat("direction", _direction);
        }
        
        // Helper method for setting the animation controller state and clearing the trap deployment
        // markers depending on if the player has touched the ground or not
        private void SetGroundedStatus(bool isGrounded)
        {
            _isGrounded = isGrounded;
            _animator.SetBool("is_grounded", _isGrounded);

            if (isGrounded) return;
            
            // Left the ground, so trap deployment isn't possible anymore
            ClearTrapDeployment();
            _isSelectingTileSFX = false;
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
            _soundsController.OnHenJump();

            // Left the ground, so trap deployment isn't possible anymore
            SetGroundedStatus(false);
        }

        private void DeployTrap(InputAction.CallbackContext obj)
        {
            // Left out of State pattern to allow this during movement
            if(!_canDeploy || _previousTilePositions.Count < 1)
            {
                Debug.Log("Can't Deploy Trap!");
                return;
            }

            var trapToSpawn = Traps[_currentTrapIndex];
            if (!CoinManager.Instance.CanAfford(trapToSpawn.Cost)) {
                Debug.Log("Can't afford the trap!");
                return;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = _trapTileMap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = _direction < 0
                ? trapToSpawn.GetLeftSpawnPoint(deploymentOrigin)
                : trapToSpawn.GetRightSpawnPoint(deploymentOrigin);

            var trapGameObject = Instantiate(trapToSpawn.gameObject);
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, _trapCanvas, 
                _previousTilePositions.ToArray(), _soundsController);
            
            // TODO: For testing purposes, remove this afterward!
            CoinManager.Instance.EarnCoins(5);
            
            CoinManager.Instance.ConsumeCoins(trapToSpawn.Cost);
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

        private void EnterIdleState()
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
        
        // -------------- Collision ---------------
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isGrounded) return;

            for (var i = 0; i < collision.GetContacts(collision.contacts); i++)
            {
                var contactPosition = (Vector3)collision.GetContact(i).point;
                var contactTilePosition = _groundTileMap.WorldToCell(contactPosition);

                if (!IsTileOfType<CustomGroundRuleTile>(_groundTileMap, contactTilePosition)) continue;
                
                SetGroundedStatus(true);
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

            if (!IsTileOfType<CustomGroundRuleTile>(_groundTileMap, contactTilePosition)) return;
            
            SetGroundedStatus(false);
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
    }
}
