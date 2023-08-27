using Audio;
using Managers;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Player {
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
        protected InputEventTrace InputRecorder;

        // ------------- Trap Deployment ------------
        private float _direction = -1;
        private bool _isGrounded = true;
        public UnityEvent<int> OnSelectedTrapIndexChanged;
        public UnityEvent<int> OnTrapDeployed { get; private set; }

        private TrapController _trapController;

        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;
        
        // --------------- Collision -----------------
        private ContactPoint2D _lastContact;

        // --------------- Bookkeeping ---------------
        private Animator _animator;

        private PlayerInputActions _playerInputActions;
        private const string BaseFolder = "InputRecordings";

        protected virtual void Awake()
        {
            this.RBody = this.GetComponent<Rigidbody2D>();
            this._animator = this.GetComponent<Animator>();
            this._trapController = this.GetComponent<TrapController>();
            this._soundsController = this.GetComponent<PlayerSoundsController>();
            this.InputRecorder = new InputEventTrace();

            _idle = new IdleState();
            _moving = new MovingState(this.Speed);
            _gameOver = new GameOverState();

            this._state = _idle;
            this.OnPlayerStateChanged = new UnityEvent<IPlayerState, float, float, float>();
            this.OnTrapDeployed = new UnityEvent<int>();
            this.OnSelectedTrapIndexChanged = new UnityEvent<int>();
            
            _animator.SetBool("is_grounded", _isGrounded);
        }

        private void Start()
        {
            this._jumpCommand = new JumpCommand(this);
            this._deployCommand = new DeployCommand(this);

            GameManager.Instance.OnHenWon.AddListener(this.StopSession);
            GameManager.Instance.OnHenLost.AddListener(this.StopSession);

            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            this.OnSelectedTrapIndexChanged?.Invoke(this._trapController.CurrentTrapIndex);
            this.OnEnable();
        }

        protected virtual void FixedUpdate()
        {
            var directionInput = this._playerInputActions.Player.Move.ReadValue<float>();
            this._direction = directionInput != 0 ? directionInput : this._direction;

            // Set animation values
            this.SetAnimatorValues(directionInput);

            // Delegate movement behaviour to state classes
            this._state.Act(this, this._direction);

            // Check trap deployment eligibility
            this._trapController.SurveyTrapDeployment(this._isGrounded, this._direction);
        }

        public void GameOver()
        {
            this._soundsController.OnHenDeath();

            var prevState = this._state;
            this._state.OnExit(_gameOver);
            this._state = _gameOver;
            this._state.OnEnter(prevState);

            var currentPos = this.transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
        }

        //========================================
        // Getters
        //========================================

        public IPlayerState GetPlayerState()
        {
            return this._state;
        }

        // Retrieves the cost of the current trap selected
        public int GetTrapCost()
        {
            return this._trapController.GetCurrentTrapCost();
        }

        public void DisablePlayerInput() {
            this._playerInputActions.Disable();
        }

        //========================================
        // Animator
        //========================================

        private void SetAnimatorValues(float inputDirection)
        {
            this._animator.SetFloat("speed", Mathf.Abs(inputDirection));
            this._animator.SetFloat("direction", this._direction);
        }
        
        // Helper method for setting the animation controller state and clearing the trap deployment
        // markers depending on if the player has touched the ground or not
        public void SetGroundedStatus(bool isGrounded)
        {
            this._isGrounded = isGrounded;
            this._animator.SetBool("is_grounded", this._isGrounded);

            if (isGrounded) return;
            
            // Left the ground, so trap deployment isn't possible anymore
            this._trapController.DisableTrapDeployment();
        }

        //========================================
        // Input
        //========================================

        private void Idle(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = this._state;
            this._state.OnExit(_idle);
            this._state = _idle;
            this._state.OnEnter(prevState);

            var currentPos = this.transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
        }

        private void Move(InputAction.CallbackContext obj)
        {
            // Cache previous state and call OnExit and OnEnter
            var prevState = this._state;
            this._state.OnExit(_moving);
            this._state = _moving;
            this._state.OnEnter(prevState);

            var currentPos = this.transform.position;
            this.OnPlayerStateChanged?.Invoke(this._state, currentPos.x, currentPos.y, currentPos.z);
        }

        private void Jump(InputAction.CallbackContext obj)
        {
            this.ExecuteCommand(this._jumpCommand);
        }

        private void DeployTrap(InputAction.CallbackContext obj)
        {
            this.ExecuteCommand(this._deployCommand);
        }

        // Test functions to switch between test traps
        private void SetTrap1(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 0);
            this.ExecuteCommand(command);
        }

        private void SetTrap2(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 1);
            this.ExecuteCommand(command);
        }

        private void SetTrap3(InputAction.CallbackContext obj)
        {
            var command = new SetTrapCommand(this, 2);
            this.ExecuteCommand(command);
        }

        //========================================
        // Pawn Inherited Methods
        //========================================

        public override void Jump()
        {
            // Left out of State pattern to allow this during movement
            this.RBody.AddForce(Vector2.up * this.JumpingForce);
            this._isGrounded = false;

            this._animator.SetBool("is_grounded", this._isGrounded);

            this._soundsController.OnHenJump();

            // Left the ground, so trap deployment isn't possible anymore
            this._trapController.DisableTrapDeployment();
        }

        public override void DeployTrap()
        {
            // Delegate deployment to TrapController for better encapsulation and efficiency
            if (_trapController.DeployTrap(_direction, out var trapIndex)) this.OnTrapDeployed?.Invoke(trapIndex);
        }

        public override void ChangeTrap(int trapIndex)
        {
            // Delegate setting trap to TrapController for better encapsulation and efficiency
            this._trapController.ChangeTrap(trapIndex);
            this.OnSelectedTrapIndexChanged?.Invoke(trapIndex);
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
            this.InputRecorder.Enable();
            print("Start Recording");

            this.EnableControls();
        }

        protected virtual void StopSession(string message)
        {
            this.InputRecorder.Disable();
            print("Stop Recording");

            // Create a new playtest session recording file
            this.CreateRecordingFile();

            // Prevent memory leaks!
            this.InputRecorder.Dispose();
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

            this.InputRecorder.WriteTo(path + fileName);
        }
        
        //========================================
        // Collisions
        //========================================
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (this._isGrounded) return;

            for (var i = 0; i < collision.GetContacts(collision.contacts); i++)
            {
                var contactPosition = (Vector3)collision.GetContact(i).point + (Vector3.down * .15f);
                
                if (!this._trapController.CheckForGroundTile(contactPosition)) continue;
                
                this.SetGroundedStatus(true);
                return;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            this._lastContact = collision.GetContact(0);
        }

        // Called one frame after the collision, so fetch contact point from the last frame;
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!this._isGrounded) return;
            
            var contactPosition = (Vector3)this._lastContact.point + (Vector3.down * .05f);
             
            if (!this._trapController.CheckForGroundTile(contactPosition)) return;
            
            this.SetGroundedStatus(false);
        }

        //========================================
        // Control bindings, etc.
        //========================================

        private void OnEnable() {
            // OnEnable called before Start
            // PlayerInputController.Instance and this._playerInputController may be uninitialized
            // when the scene is just started

            if (this._playerInputActions == null)
                return;

            this.EnableControls();
        }

        private void EnableControls()
        {
            this._playerInputActions.Player.Move.performed += this.Move;
            this._playerInputActions.Player.Move.canceled += this.Idle;
            this._playerInputActions.Player.Jump.performed += this.Jump;
            this._playerInputActions.Player.PlaceTrap.performed += this.DeployTrap;

            // Test functions to set the traps
            this._playerInputActions.Player.SetTrap1.performed += this.SetTrap1;
            this._playerInputActions.Player.SetTrap2.performed += this.SetTrap2;
            this._playerInputActions.Player.SetTrap3.performed += this.SetTrap3;
        }

        private void OnDisable() {
            this.DisableControls();
        }

        private void DisableControls()
        {
            this._playerInputActions.Player.Move.performed -= this.Move;
            this._playerInputActions.Player.Move.canceled -= this.Idle;
            this._playerInputActions.Player.Jump.performed -= this.Jump;
            this._playerInputActions.Player.PlaceTrap.performed -= this.DeployTrap;
            this._playerInputActions.Player.SetTrap1.performed -= this.SetTrap1;
            this._playerInputActions.Player.SetTrap2.performed -= this.SetTrap2;
            this._playerInputActions.Player.SetTrap3.performed -= this.SetTrap3;
        }
    }
}
