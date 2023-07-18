using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Player.Input.Commands;
using Traps;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player.Input
{
    //*******************************************************************************************
    // PlayerController
    //*******************************************************************************************
    /// <summary>
    /// A class to handle the brunt of player input: Contains the implementation for the
    /// character movement, jumping, changing and deployment of traps, as well as player
    /// death with associated animations and sound. Works hand-in-hand with the
    /// TrapController class made to separate the trap searching and tiles painting logic.
    /// </summary>
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

        // ------------- Trap Deployment ------------
        // The canvas to spawn trap UI
        [SerializeField] private Canvas _trapCanvas;
        
        private float _direction = -1;
        private bool _isGrounded = true;
        public UnityEvent<int> OnSelectedTrapIndexChanged;

        private TrapController _trapController;

        // ------------- Sound Effects ---------------
        public AK.Wwise.Event StartBuildEvent;
        public AK.Wwise.Event StopBuildEvent;
        public AK.Wwise.Event BuildCompleteEvent;
        public AK.Wwise.Event HenDeathEvent;
        public AK.Wwise.Event HenFlapEvent;

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

            _trapController = GetComponent<TrapController>();

            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            this.OnSelectedTrapIndexChanged?.Invoke(_trapController.CurrentTrapIndex);
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
            _trapController.SurveyTrapDeployment(_isGrounded, _direction);
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
        
        //========================================
        // Getters
        //========================================
        
        public IPlayerState GetPlayerState()
        {
            return _state;
        }

        public void DisablePlayerInput() {
            this._playerInputActions.Disable();
        }

        //========================================
        // Animator
        //========================================
        
        private void SetAnimatorValues(float inputDirection)
        {
            _animator.SetFloat("speed", Mathf.Abs(inputDirection));
            _animator.SetFloat("direction", _direction);
        }

        //========================================
        // Input
        //========================================
        
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
        
        //========================================
        // Pawn Inherited Methods
        //========================================
        
        public override void Jump()
        {
            // Left out of State pattern to allow this during movement
            RBody.AddForce(Vector2.up * JumpingForce);
            _isGrounded = false;

            _animator.SetBool("is_grounded", _isGrounded);

            HenFlapEvent.Post(gameObject);

            // Left the ground, so trap deployment isn't possible anymore
            _trapController.ClearTrapDeployment();
        }
        
        public override void DeployTrap()
        {
            // Left out of State pattern to allow this during movement
            if(!_trapController.CanDeploy || _trapController.PreviousTilePositions.Count < 1)
            {
                // TODO: Make an animation for this!
                print("Can't Deploy Trap!");
                return;
            }

            StartCoroutine(PlayBuildSoundForDuration(.3f));

            var trapToSpawn = _trapController.GetCurrentTrap();
            var trapScript = trapToSpawn.GetComponent<Trap>();
            if (!CoinManager.Instance.CanAfford(trapScript.Cost)) {
                print("Can't afford the trap!");
                return;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = _trapController.TileMap.CellToWorld(_trapController.PreviousTilePositions[0]);
            var spawnPosition = _direction < 0
                ? trapScript.GetLeftSpawnPoint(deploymentOrigin)
                : trapScript.GetRightSpawnPoint(deploymentOrigin);

            GameObject trapGameObject = Instantiate(trapToSpawn.gameObject);
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, _trapCanvas,
                StartBuildEvent, StopBuildEvent, BuildCompleteEvent);
            _trapController.IsColliding = true;
            CoinManager.Instance.ConsumeCoins(trapScript.Cost);
        }
        
        public override void ChangeTrap(int trapIndex)
        {
            _trapController.CurrentTrapIndex = trapIndex;
            this.OnSelectedTrapIndexChanged?.Invoke(trapIndex);

            _trapController.ClearTrapDeployment();
        }
        
        //========================================
        // Command
        //========================================
        
        // Method to execute any given command (interfaced) and record it to a list to replay
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            // Record the command to the log of previous commands
            prevCommands.Add(command);
        }
        
        //========================================
        // Music
        //========================================
        
        private IEnumerator PlayBuildSoundForDuration(float durationInSeconds)
        {
            StartBuildEvent.Post(gameObject);
            yield return new WaitForSeconds(durationInSeconds);
            StopBuildEvent.Post(gameObject);
        }

        //========================================
        // Unity Physics / etc.
        //========================================
        
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
