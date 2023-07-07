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
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        // --------------- Player State --------------
        private static IdleState _idle;
        private static MovingState _moving;
        private static DeployingState _deploying;
        private static GameOverState _gameOver;
        private IPlayerState _state;
        public UnityEvent<IPlayerState> OnPlayerStateChanged { get; private set; }

        // For movement testing, allow speeds to be set through the editor
        [Header("State speed parameters")]
        [SerializeField] private float _speed, _jumpingForce;

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
            _deploying = new DeployingState();
            _gameOver = new GameOverState();

            _state = _idle;
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
            _state.Act(_rBody, _playerInputActions.Player.Move.ReadValue<float>(), EnterIdleState);

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
        
        void Move(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_moving);
            _state = _moving;
            _state.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state);
        }

        void Jump(InputAction.CallbackContext obj)
        {
            _rBody.AddForce(Vector2.up * _jumpingForce);
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
        }

        private void OnDisable() {
            this._playerInputActions.Player.Move.performed -= Move;
            this._playerInputActions.Player.Move.canceled -= Idle;
            this._playerInputActions.Player.Jump.performed -= Jump;
            this._playerInputActions.Player.PlaceTrap.performed -= DeployTrap;
        }
    }
}