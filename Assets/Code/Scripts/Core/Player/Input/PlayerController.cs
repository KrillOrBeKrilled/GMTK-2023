using System;
using System.IO;
using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Core.Commands;
using KrillOrBeKrilled.Core.Commands.Interfaces;
using KrillOrBeKrilled.Input;
using KrillOrBeKrilled.Traps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

//*******************************************************************************************
// PlayerController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Handles the brunt of the player input. Works hand-in-hand with the
    /// <see cref="TrapController"/> class made to separate the trap searching and tile
    /// painting logic.
    /// </summary>
    /// <remarks>
    /// Contains the implementation for the character movement, jumping,
    /// changing and deployment of traps, as well as player death with associated
    /// animations and sound.
    /// </remarks>
    public class PlayerController : Pawn, IDamageable, ITrapBuilder {
        // --------------- Player State --------------
        public enum State {
            Idle,
            Moving,
            GameOver,
            Jumping,
            Gliding,
        }

        private IdleState _idle;
        private MovingState _moving;
        private GameOverState _gameOver;
        private JumpingState _jumpingState;
        private GlidingState _glidingState;

        private IPlayerState _state;
        private Dictionary<State, IPlayerState> _states;

        [Tooltip("Tracks when the player state changes.")]
        internal UnityEvent<IPlayerState, Vector3> OnPlayerStateChanged { get; private set; }

        // ----------------- Command -----------------
        // Stateless commands; can be copied in the list of previous commands
        private ICommand _deployCommand;

        // TODO: Consider undoing and redoing actions?

        // ---------------- Replaying ----------------
        protected InputEventTrace InputRecorder;

        // ------------- Trap Deployment ------------
        private float _direction = -1;
        public bool IsGrounded { get; private set; } = true;
        public bool IsFalling => this.RBody.velocity.y < -0.1f;

        [Tooltip("Tracks when a new trap is selected.")]
        internal UnityEvent<Trap> OnSelectedTrapIndexChanged;
        [Tooltip("Tracks when a trap has been deployed.")]
        internal UnityEvent<Trap> OnTrapDeployed { get; private set; }

        private TrapController _trapController;

        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;

        // --------------- Collision -----------------
        private ContactPoint2D _lastContact;

        // --------------- Bookkeeping ---------------
        private Animator _animator;

        private PlayerInputActions _playerInputActions;
        private const string BaseFolder = "InputRecordings";

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        protected virtual void Awake() {
            this.RBody = this.GetComponent<Rigidbody2D>();
            this._animator = this.GetComponent<Animator>();
            this._trapController = this.GetComponent<TrapController>();
            this._soundsController = this.GetComponent<PlayerSoundsController>();
            this.InputRecorder = new InputEventTrace();

            this._idle = new IdleState(this);
            this._moving = new MovingState(this);
            this._gameOver = new GameOverState();
            this._jumpingState = new JumpingState(this);
            this._glidingState = new GlidingState(this);

            this._states = new Dictionary<State, IPlayerState>() {
                { State.Idle, this._idle },
                { State.Moving, this._moving },
                { State.GameOver, this._gameOver },
                { State.Jumping, this._jumpingState },
                { State.Gliding, this._glidingState}
            };

            this.ChangeState(State.Idle);

            this.OnPlayerStateChanged = new UnityEvent<IPlayerState, Vector3>();
            this.OnTrapDeployed = new UnityEvent<Trap>();
            this.OnSelectedTrapIndexChanged = new UnityEvent<Trap>();

            _animator.SetBool("is_grounded", this.IsGrounded);
        }

        /// <remarks> Invokes the <see cref="OnSelectedTrapIndexChanged"/> event. </remarks>
        private void Start() {
            this._deployCommand = new DeployCommand(this);

            // Need this due to race condition during scene Awake->OnEnable calls
            this._playerInputActions = PlayerInputController.Instance.PlayerInputActions;
            this.OnEnable();
        }

        protected virtual void FixedUpdate() {
            // Read Input
            Vector2 moveVectorInput = this._playerInputActions.Player.Move.ReadValue<Vector2>();
            float moveInput = moveVectorInput.x;
            // Must be read as float.
            // Reading value from jump returns 0 for NOT pressed, 1 otherwise.
            bool jumpTriggered = this._playerInputActions.Player.Jump.ReadValue<float>() > 0.2f;

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._direction = moveInput > 0 ? 1 : -1;
            }

            // Set animation values
            this.SetAnimatorValues(moveInput);

            // Delegate behaviour to the current state
            this._state.Act(moveInput, jumpTriggered);

            // Check trap deployment eligibility
            this._trapController.SurveyTrapDeployment(this.IsGrounded, this._direction);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (this.IsGrounded) {
                return;
            }

            for (var i = 0; i < collision.GetContacts(collision.contacts); i++) {
                var contactPosition = (Vector3)collision.GetContact(i).point + (Vector3.down * .15f);

                if (!this._trapController.CheckForGroundTile(contactPosition)) {
                    continue;
                }

                this.SetGroundedStatus(true);
                return;
            }
        }

        private void OnCollisionStay2D(Collision2D collision) {
            this._lastContact = collision.GetContact(0);
        }

        // Called one frame after the collision, so fetch contact point from the last frame;
        private void OnCollisionExit2D(Collision2D collision) {
            if (!this.IsGrounded) {
                return;
            }

            var contactPosition = (Vector3)this._lastContact.point + (Vector3.down * .05f);

            if (!this._trapController.CheckForGroundTile(contactPosition)) {
                return;
            }

            this.SetGroundedStatus(false);
        }

        private void OnEnable() {
            // OnEnable called before Start
            // PlayerInputController.Instance and this._playerInputController may be uninitialized
            // when the scene is just started

            if (this._playerInputActions == null)
                return;

            this.EnableControls();
        }

        private void OnDisable() {
            this.DisableControls();
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        #region IDamageable Implementations

        public void ApplySpeedPenalty(float penalty) {}

        /// <summary>
        /// Changes the current <see cref="IPlayerState"/> to the death state and plays associated SFX.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnPlayerStateChanged"/> event. </remarks>
        public void Die() {
            this._soundsController.OnHenDeath();

            var prevState = this._state;
            this._state.OnExit(_gameOver);
            this._state = _gameOver;
            this._state.OnEnter(prevState);

            this.OnPlayerStateChanged?.Invoke(this._state, this.transform.position);
        }

        // TODO: Do we want the player to have a health bar?
        public int GetHealth() {
            return -1;
        }

        public void ResetSpeedPenalty() {}

        public void TakeDamage(int amount) {}

        public void ThrowActorBack(float stunDuration, float throwForce) {}

        #endregion

        #region ITrapBuilder Implementations

        /// <summary>
        /// Checks that the player is Idle.
        /// </summary>
        /// <returns> If the player is currently in an idle state. </returns>
        public bool CanBuildTrap() {
            return this._state is IdleState;
        }

        /// <summary>
        /// Sets the animation controller state and clears the trap deployment markers depending on whether the
        /// player has touched the ground or not.
        /// </summary>
        /// <param name="isGrounded"> If the player is currently touching the ground. </param>
        public void SetGroundedStatus(bool isGrounded) {
            this.IsGrounded = isGrounded;
            this._animator.SetBool("is_grounded", this.IsGrounded);

            if (isGrounded) {
                return;
            }

            // Left the ground, so trap deployment isn't possible anymore
            this._trapController.DisableTrapDeployment();
        }

        /// <summary>
        /// Changes the current <see cref="IPlayerState"/> to the <paramref name="newState"/>.
        /// </summary>
        /// <param name="newState"> the new state to change to. </param>
        /// <remarks> Invokes the <see cref="OnPlayerStateChanged"/> event. </remarks>
        public void ChangeState(State newState) {
            IPlayerState nextState = this._states[newState];
            IPlayerState prevState = this._state;
            this._state?.OnExit(_moving);
            this._state = nextState;
            this._state?.OnEnter(prevState);
            this.OnPlayerStateChanged?.Invoke(this._state, this.transform.position);
        }

        public void OnJumpStart() {
            this.IsGrounded = false;
            this._animator.SetBool("is_grounded", this.IsGrounded);

            // Left the ground, so trap deployment isn't possible anymore
            this._trapController.DisableTrapDeployment();
        }

        public void PlayJumpSound() {
            this._soundsController.OnHenJump();
        }

        #endregion

        #region Pawn Inherited Methods

        /// <inheritdoc cref="Pawn.ChangeTrap"/>
        /// <remarks>
        /// Delegates trap selection execution to the <see cref="TrapController"/>.
        /// Invokes the <see cref="OnSelectedTrapIndexChanged"/> event.
        /// </remarks>
        public override void ChangeTrap(Trap trap) {
            // Delegate setting trap to TrapController for better encapsulation and efficiency
            this._trapController.ChangeTrap(trap);
            this.OnSelectedTrapIndexChanged?.Invoke(trap);
        }

        /// <inheritdoc cref="Pawn.DeployTrap"/>
        /// <remarks>
        /// Delegates trap deployment execution to <see cref="TrapController.DeployTrap"/>.
        /// Invokes the <see cref="OnTrapDeployed"/> event.
        /// </remarks>
        public override void DeployTrap() {
            if (_trapController.DeployTrap(_direction, out Trap trap)) {
                this.OnTrapDeployed?.Invoke(trap);
            }
        }

        #endregion

        public void SetTrap(Trap selectedTrap) {
            var command = new SetTrapCommand(this, selectedTrap);
            this.ExecuteCommand(command);
        }

        #endregion

        //========================================
        // Protected Methods
        //========================================

        #region Protected Methods

        /// <summary>
        /// Stops recording play input via the <see cref="InputEventTrace"/>, creates a file for the recorded input,
        /// and frees the memory used for the recording process.
        /// </summary>
        /// <remarks>
        /// Subscribed to the <see cref="GameManager.OnHenWon"/> and <see cref="GameManager.OnHenLost"/> events.
        /// </remarks>
        protected virtual void StopSession(string message) {
            this.InputRecorder.Disable();
            this.CreateRecordingFile();

            // Prevent memory leaks!
            this.InputRecorder.Dispose();
        }

        #endregion

        //========================================
        // Internal Methods
        //========================================

        #region Internal Methods

        #region Getters

        /// <summary>
        /// Retrieves the current <see cref="IPlayerState"/> stored in <see cref="_state"/>.
        /// </summary>
        /// <returns> The <see cref="IPlayerState"/> that is currently being executed. </returns>
        internal IPlayerState GetPlayerState() {
            return this._state;
        }

        /// <summary>
        /// Retrieves the cost of the current selected trap through the <see cref="TrapController"/>.
        /// </summary>
        /// <returns> The cost of the current selected trap. </returns>
        internal int GetTrapCost() {
            return this._trapController.GetCurrentTrapCost();
        }

        #endregion

        /// <summary>
        /// Disables the retrieval of controller input in <see cref="PlayerInputActions"/>.
        /// </summary>
        internal void DisablePlayerInput() {
            this._playerInputActions.Disable();
        }

        /// <summary>
        /// Executes an <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command"> The <see cref="ICommand"/> to be executed. </param>
        /// <remarks>
        /// Can be extended to record each command to a list to implement redo/undo logic, especially sorted by type.
        /// </remarks>
        internal void ExecuteCommand(ICommand command) {
            command.Execute();
        }

        /// <summary>
        /// Sets up all listeners to operate the <see cref="PlayerController"/>.
        /// </summary>
        /// <param name="gameManager"> Provides events related to the game state to subscribe to. </param>
        internal void Initialize(GameManager gameManager) {
            gameManager.OnHenWon.AddListener(this.StopSession);
            gameManager.OnHenLost.AddListener(this.StopSession);

            this.OnSelectedTrapIndexChanged?.Invoke(this._trapController.CurrentTrap);
        }

        /// <summary>
        /// Begins recording all player input with timestamps via the <see cref="InputEventTrace"/> and enables
        /// all controls from the <see cref="PlayerInputActions"/>.
        /// </summary>
        internal virtual void StartSession() {
            this.InputRecorder.Enable();

            this.EnableControls();
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        #region Input

        /// <summary>
        /// Executes the <see cref="DeployCommand"/>.
        /// </summary>
        private void DeployTrap(InputAction.CallbackContext obj) {
            this.ExecuteCommand(this._deployCommand);
        }

        #endregion

        /// <summary>
        /// Helper method for creating a file for the player input recording based on platform the game was played on.
        /// </summary>
        /// <remarks>
        /// The recording files will be stored in <b>Assets > InputRecordings</b> within the project
        /// hierarchy. For a build, the recordings can be found in <b>Henchman_Data > InputRecordings</b>.
        /// </remarks>
        private void CreateRecordingFile() {
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

        /// <summary>
        /// Unsubscribes all the input methods from the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void DisableControls() {
            this._playerInputActions.Player.PlaceTrap.performed -= this.DeployTrap;
        }

        /// <summary>
        /// Subscribes all the input methods to the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void EnableControls() {
            this._playerInputActions.Player.PlaceTrap.performed += this.DeployTrap;
        }

        /// <summary>
        /// Adjusts the <see cref="Animator"/> speed and direction values.
        /// </summary>
        /// <param name="inputDirection"> The vector x-value associated with the player's current movement. </param>
        private void SetAnimatorValues(float inputDirection) {
            this._animator.SetFloat("speed", Mathf.Abs(inputDirection));
            this._animator.SetFloat("direction", this._direction);
        }

        #endregion
    }
}
