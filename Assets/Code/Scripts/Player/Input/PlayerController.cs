using System;
using System.IO;
using Code.Scripts.Player.Input.Commands;
using Traps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
        public UnityEvent<IPlayerState, float, float, float> OnPlayerStateChanged { get; private set; }
        
        // ----------------- Command -----------------
        // Stateless commands; can be copied in the list of previous commands
        private ICommand _jumpCommand;
        private ICommand _deployCommand;
        
        // TODO: Consider undoing and redoing actions?

        // ---------------- Replaying ----------------
        protected InputEventTrace _inputRecorder;

        // ------------- Trap Deployment ------------
        // The canvas to spawn trap UI
        [SerializeField] private Canvas _trapCanvas;
        
        private float _direction = -1;
        private bool _isGrounded = true;
        public UnityEvent<int> OnSelectedTrapIndexChanged;
        public UnityEvent<int> OnTrapDeployed { get; private set; }

        private TrapController _trapController;

        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;

        // --------------- Bookkeeping ---------------
        private Animator _animator;

        private PlayerInputActions _playerInputActions;
        private const string BaseFolder = "InputRecordings";

        protected virtual void Awake()
        {
            RBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _trapController = GetComponent<TrapController>();
            _soundsController = GetComponent<PlayerSoundsController>();
            _inputRecorder = new InputEventTrace();

            _idle = new IdleState();
            _moving = new MovingState(Speed);
            _gameOver = new GameOverState();

            _state = _idle;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState, float, float, float>();
            this.OnTrapDeployed = new UnityEvent<int>();
            this.OnSelectedTrapIndexChanged = new UnityEvent<int>();
        }

        private void Start()
        {
            _jumpCommand = new JumpCommand(this);
            _deployCommand = new DeployCommand(this);

            GameManager.Instance.OnHenWon.AddListener(this.StopSession);
            GameManager.Instance.OnHenLost.AddListener(this.StopSession);

            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            this.OnSelectedTrapIndexChanged?.Invoke(_trapController.CurrentTrapIndex);
            OnEnable();
        }

        protected virtual void FixedUpdate()
        {
            var directionInput = _playerInputActions.Player.Move.ReadValue<float>();
            _direction = directionInput != 0 ? directionInput : _direction;

            // Set animation values
            SetAnimatorValues(directionInput);

            // Delegate movement behaviour to state classes
            _state.Act(this, _direction);

            // Check trap deployment eligibility
            _trapController.SurveyTrapDeployment(_isGrounded, _direction);
        }
        
        public void GameOver()
        {
            _soundsController.OnHenDeath();
            
            var prevState = _state;
            _state.OnExit(_gameOver);
            _state = _gameOver;
            _state.OnEnter(prevState);

            var currentPos = transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
        }

        //========================================
        // Getters
        //========================================
        
        public IPlayerState GetPlayerState()
        {
            return _state;
        }
        
        // Retrieves the cost of the current trap selected
        public int GetTrapCost()
        {
            return _trapController.GetCurrentTrapCost();
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
            
            var currentPos = transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
        }

        private void Move(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = _state;
            _state.OnExit(_moving);
            _state = _moving;
            _state.OnEnter(prevState);
            
            var currentPos = transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
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

            _soundsController.OnHenJump();

            // Left the ground, so trap deployment isn't possible anymore
            _trapController.ClearTrapDeployment();
            _trapController.IsSelectingTileSFX = false;
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
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, _trapCanvas, _soundsController);
            _trapController.IsColliding = true;
            
            CoinManager.Instance.ConsumeCoins(trapScript.Cost);
            this.OnTrapDeployed?.Invoke(_trapController.CurrentTrapIndex);
            _soundsController.OnTileSelectConfirm();
        }
        
        public override void ChangeTrap(int trapIndex)
        {
            _trapController.CurrentTrapIndex = trapIndex;
            this.OnSelectedTrapIndexChanged?.Invoke(trapIndex);

            _trapController.ClearTrapDeployment();
            _trapController.IsSelectingTileSFX = false;
        }
        
        //========================================
        // Command
        //========================================
        
        // Method to execute any given command (interfaced) and record it to a list to replay
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
        }
        
        //========================================
        // Recording
        //========================================

        public virtual void StartSession()
        {
            _inputRecorder.Enable();
            print("Start Recording");
            
            EnableControls();
        }
        
        protected virtual void StopSession(string message)
        {
            _inputRecorder.Disable();
            print("Stop Recording");
            
            // Create a new playtest session recording file
            CreateRecordingFile();
            
            // Prevent memory leaks!
            _inputRecorder.Dispose();
        }

        private void CreateRecordingFile()
        {
            var fileName = $"Playtest {DateTime.Now:MM-dd-yyyy HH-mm-ss}.txt";
            
            // From https://stackoverflow.com/questions/70715187/unity-read-and-write-to-txt-file-after-build
#if UNITY_EDITOR
            var path = Application.dataPath + $"/{BaseFolder}/" ;
            var path1 = Application.dataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#elif UNITY_ANDROID
            var path = Application.persistentDataPath + $"/{BaseFolder}/";
            var path1 = Application.persistentDataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#elif UNITY_IPHONE
            var path = Application.persistentDataPath + $"/{BaseFolder}/";
            var path1 = Application.persistentDataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#else
            var path = Application.dataPath + $"/{BaseFolder}/";
            var path1 = Application.dataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#endif
            
            _inputRecorder.WriteTo(path + fileName);
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
                
            EnableControls();
        }

        private void EnableControls()
        {
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
            DisableControls();
        }

        private void DisableControls()
        {
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
