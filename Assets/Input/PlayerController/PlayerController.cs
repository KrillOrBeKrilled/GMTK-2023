using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    /// <summary>
    /// Class to handle character controls
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // --------------- Player State --------------
        private static IdleState _idle;
        private static MovingState _moving;
        private static JumpingState _jumping;
        private static DeployingState _deploying;
        private static GameOverState _gameOver;
        private IPlayerState _state;
        public UnityEvent<IPlayerState> OnPlayerStateChanged { get; private set; }

        // For movement testing, allow speeds to be set through the editor
        [Header("State speed parameters")]
        [SerializeField] private float _speed, _jumpingForce;
        [SerializeField] private float _idleToMoveBlendDuration, _moveToIdleBlendDuration;
        private float _direction;

        // ----------------- Health ------------------
        // BRAINSTORMING: Do we want to simulate player health?

        // --------------- Bookkeeping ---------------
        private Rigidbody2D _rBody;
        public Animator _animator;
        private float _startXPos;

        private PlayerInputActions _playerInputActions;

        void Awake()
        {
            _rBody = GetComponent<Rigidbody2D>();

            _idle = new IdleState(_speed, _moveToIdleBlendDuration);
            _moving = new MovingState(_speed, _idleToMoveBlendDuration);
            _jumping = new JumpingState(_jumpingForce);
            _deploying = new DeployingState();
            _gameOver = new GameOverState();

            _state = _idle;

            _startXPos = transform.position.x;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState>();
        }

        void Start() {
            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            OnEnable();
        }
        
        void FixedUpdate()
        {
            // Delegate movement behaviour to state classes
            _state.Act(transform, _rBody, _direction);

            // Set animation values
            SetAnimatorValues();
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
        void Idle(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_idle);
            _state = _idle;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }
        
        void MoveForward(InputAction.CallbackContext obj)
        {
            _direction = 1f;
            
            // Cache previous state and call OnExit and OnEnter
            ChangeToMoveState();
        }
        
        void MoveBack(InputAction.CallbackContext obj)
        {
            _direction = -1f;
            
            // Cache previous state and call OnExit and OnEnter
            ChangeToMoveState();
        }
        
        void Jump(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_jumping);
            _state = _jumping;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }
        
        void DeployTrap(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_deploying);
            _state = _deploying;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
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
        
        private void ChangeToMoveState()
        {
            var prevState = _state;
            _state.OnExit(_moving);
            _state = _moving;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }

        private void OnEnable() {
            // OnEnable called before Start
            // PlayerInputController.Instance and this._playerInputController may be uninitialized
            // when the scene is just started

            if (this._playerInputActions == null)
                return;

            this._playerInputActions.Player.MoveBackwards.performed += MoveBack;
            this._playerInputActions.Player.MoveForward.performed += MoveForward;
            this._playerInputActions.Player.Jump.performed += Jump;
            this._playerInputActions.Player.PlaceTrap.performed += DeployTrap;
            this._playerInputActions.Player.MoveBackwards.canceled += Idle;
            this._playerInputActions.Player.MoveForward.canceled += Idle;
        }

        private void OnDisable() {
            this._playerInputActions.Player.MoveBackwards.performed -= MoveBack;
            this._playerInputActions.Player.MoveForward.performed -= MoveForward;
            this._playerInputActions.Player.Jump.performed -= Jump;
            this._playerInputActions.Player.PlaceTrap.performed -= DeployTrap;
            this._playerInputActions.Player.MoveBackwards.canceled -= Idle;
            this._playerInputActions.Player.MoveForward.canceled -= Idle;
        }
    }
}