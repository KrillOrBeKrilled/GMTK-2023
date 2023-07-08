using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
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

        private float _direction;
        
        // ------------- Trap Deployment -------------
        [SerializeField] private List<GameObject> _trapPrefabs;
        private int _currentTrapIndex = 0;
        
        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private Transform _leftDeployTransform, _rightDeployTransform;

        private readonly List<Vector3Int> _previousTilePositions = new List<Vector3Int>();
        private bool _isGrounded = true, _canDeploy = false;

        // ----------------- Health ------------------
        // BRAINSTORMING: Do we want to simulate player health?

        // --------------- Bookkeeping ---------------
        private Rigidbody2D _rBody;
        public Animator _animator;

        private PlayerInputActions _playerInputActions;

        void Awake()
        {
            _rBody = GetComponent<Rigidbody2D>();

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

            // Check trap deployment eligibility
            SurveyTrapDeployment();
            
            // Delegate movement behaviour to state classes
            _state.Act(_rBody, _direction, EnterIdleState);

            // Set animation values
            SetAnimatorValues();
        }

        private void SurveyTrapDeployment()
        {
            if (_isGrounded)
            {
                var deployPosition = _direction < 0 ? _leftDeployTransform.position : _rightDeployTransform.position;
                var deploymentOrigin = _tileMap.WorldToCell(deployPosition);

                if (_previousTilePositions.Count < 1 || deploymentOrigin != _previousTilePositions[0])
                {
                    // The tile changed, so flush the tint on the previous tiles
                    foreach (var previousTilePosition in _previousTilePositions)
                    {
                        _tileMap.SetColor(previousTilePosition, Color.white);
                    }
                    _previousTilePositions.Clear();

                    // Get the grid placement data for the selected prefab
                    var selectedTrapPrefab = _trapPrefabs[_currentTrapIndex].GetComponent<Traps.Trap>();
                    var prefabPoints = _direction < 0 
                            ? selectedTrapPrefab.GetLeftGridPoints()
                            : selectedTrapPrefab.GetRightGridPoints();

                    // Validate the deployment of the trap with a validation score
                    var validationScore = 0;
                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        validationScore = IsTileOfType<TrapTile>(_tileMap, deploymentOrigin + prefabOffsetPosition)
                            ? ++validationScore
                            : validationScore;
                            
                        _tileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                        _tileMap.SetColor(deploymentOrigin + prefabOffsetPosition, Color.green);
                        _previousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
                    }
                    
                    // If the validation score isn't high enough, paint the selected tiles an invalid color
                    if (!selectedTrapPrefab.IsValidScore(validationScore))
                    {
                        foreach (var previousTilePosition in _previousTilePositions)
                        {
                            _tileMap.SetColor(previousTilePosition, Color.red);
                        }
                        
                        _canDeploy = false;
                        return;
                    }

                    _canDeploy = true;
                }
            }
        }
        
        private bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase
        {
            TileBase targetTile = tilemap.GetTile(position);
            if (targetTile != null && targetTile is T) return true;

            return false;
        }
        
        private void InvalidateTrapDeployment()
        {
            foreach (Vector3Int previousTilePosition in _previousTilePositions)
            {
                _tileMap.SetColor(previousTilePosition, Color.white);
            }
            _canDeploy = false;

            // Clear the data of the previous tile
            _previousTilePositions.Clear();
        }

        // For when we put in an animator
        private void SetAnimatorValues()
        {
            
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
            
            // Left the ground, so trap deployment isn't possible anymore
            InvalidateTrapDeployment();
        }
        
        private void DeployTrap(InputAction.CallbackContext obj)
        {
            // Left out of State pattern to allow this during movement
            if(_canDeploy) Debug.Log("Deployed trap!");
        }
        
        private void SetTrap1(InputAction.CallbackContext obj)
        {
            _currentTrapIndex = 0;
            InvalidateTrapDeployment();
        }
        
        private void SetTrap2(InputAction.CallbackContext obj)
        {
            _currentTrapIndex = 1;
            InvalidateTrapDeployment();
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
            }
        }
    }
}