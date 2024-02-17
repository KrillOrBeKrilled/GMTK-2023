using System;
using KrillOrBeKrilled.Interfaces;
using KrillOrBeKrilled.Player.Commands;
using KrillOrBeKrilled.Player.PlayerStates;
using KrillOrBeKrilled.Traps;
using System.Collections;
using System.Collections.Generic;
using KrillOrBeKrilled.Traps.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// Player
//*******************************************************************************************
namespace KrillOrBeKrilled.Player {
    /// <summary>
    /// Acts as a representation of the player within the game world, acting on input
    /// received from the controller that possesses it. Works hand-in-hand with the
    /// <see cref="TrapController"/> class that handles all tilemap grid-related player
    /// actions.
    /// </summary>
    /// <remarks>
    /// Contains the implementation for the character movement, jumping,
    /// changing and deployment of traps, as well as player death with associated
    /// animations and sound.
    /// </remarks>
    public class PlayerCharacter : Pawn, IDamageable, ITrapDamageable, ITrapBuilder {

        // ------------- Receiving Input -------------
        // Support the passing of a delegate with out parameters
        public delegate void InputDelegate<T1, T2, T3>(out T1 input, out T2 output, out T3 output2);
        private InputDelegate<float, bool, bool> _gatherControllerInput;

        // --------------- Player State --------------
        public enum State {
            Idle,
            Moving,
            Dead,
            GameOver,
            Jumping,
            Gliding,
        }

        private IdleState _idleState;
        private MovingState _movingState;
        private JumpingState _jumpingState;
        private GlidingState _glidingState;
        private DeadState _deadState;
        private GameOverState _gameOverState;

        private IPlayerState _state;
        private Dictionary<State, IPlayerState> _states;

        [Tooltip("Tracks when the player state changes.")]
        public UnityEvent<IPlayerState, Vector3> OnPlayerStateChanged { get; private set; }
        public UnityEvent OnPlayerGrounded { get; private set; }
        public UnityEvent OnPlayerFalling { get; private set; }

        // ----------------- Command -----------------
        // Stateless commands; can be copied in the list of previous commands
        private ICommand _deployCommand;

        // TODO: Consider undoing and redoing actions?

        // ----------------- Flags -------------------
        private bool _isFrozen = false;
        private float _direction = -1;
        private bool _stateChangedThisFrame = false;

        // ---------- Grounded & Coyote Time --------
        public bool IsGrounded { get; private set; } = true;
        public bool IsFalling { get; private set; } = false;

        private const float CoyoteTimeDuration = 0.15f;
        private IEnumerator _coyoteTimeCoroutine = null;

        // ------------- Trap Deployment ------------
        [Tooltip("Tracks when a new trap is selected.")]
        public UnityEvent<Trap> OnSelectedTrapChanged;
        [Tooltip("Tracks when a trap has been deployed.")]
        public UnityEvent<Trap> OnTrapDeployed { get; private set; }

        private TrapController _trapController;

        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;

        // --------------- Bookkeeping ---------------
        private Animator _animator;
        private readonly int _speedKey = Animator.StringToHash("speed");
        private readonly int _directionKey = Animator.StringToHash("direction");
        private readonly int _groundedKey = Animator.StringToHash("is_grounded");

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            this.RBody = this.GetComponent<Rigidbody2D>();
            this._animator = this.GetComponent<Animator>();
            this._trapController = this.GetComponent<TrapController>();
            this._soundsController = this.GetComponent<PlayerSoundsController>();

            this._idleState = new IdleState(this);
            this._movingState = new MovingState(this);
            this._jumpingState = new JumpingState(this);
            this._glidingState = new GlidingState(this);
            this._deadState = new DeadState();
            this._gameOverState = new GameOverState(this);

            this._states = new Dictionary<State, IPlayerState>() {
                { State.Idle, this._idleState },
                { State.Moving, this._movingState },
                { State.Jumping, this._jumpingState },
                { State.Gliding, this._glidingState},
                { State.Dead, this._deadState },
                { State.GameOver, this._gameOverState },
            };

            this.ChangeState(State.Idle);

            this.OnPlayerStateChanged = new UnityEvent<IPlayerState, Vector3>();
            this.OnTrapDeployed = new UnityEvent<Trap>();
            this.OnSelectedTrapChanged = new UnityEvent<Trap>();
            this.OnPlayerGrounded = new UnityEvent();
            this.OnPlayerFalling = new UnityEvent();

            _animator.SetBool(_groundedKey, this.IsGrounded);
        }

        /// <remarks> Invokes the <see cref="OnSelectedTrapChanged"/> event. </remarks>
        private void Start() {
            this._deployCommand = new DeployCommand(this);
        }

        protected override void FixedUpdate() {
            if (this._isFrozen) {
                return;
            }

            // Read Input
            this.GatherInput(out float moveInput, out bool jumpPressed, out bool jumpPressedThisFrame);

            // Set animation values
            this.SetAnimatorValues(moveInput);

            // Check Grounded
            this.UpdateGrounded();

            // Check Falling
            this.UpdateFalling();

            // Delegate behaviour to the current state
            this._state.Act(moveInput, jumpPressed, jumpPressedThisFrame);

            // If state changed => call Act method on the new state
            if (this._stateChangedThisFrame) {
                this._state.Act(moveInput, jumpPressed, jumpPressedThisFrame);
            }

            // Check trap deployment eligibility
            this._trapController.SurveyTrapDeployment(this.IsGrounded, this._direction);

            this._stateChangedThisFrame = false;
            base.FixedUpdate();
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hero")) {
                return;
            }

            this.Die();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            float halfWidth = this.GroundedCheckBoxSize.x / 2;
            float halfHeight = this.GroundedCheckBoxSize.y / 2;
            Vector3 origin = this.GroundedCheckBoxOffset + (Vector2)this.transform.position;
            Vector3 p1 = origin + new Vector3(-halfWidth, halfHeight, 0f);
            Vector3 p2 = origin + new Vector3(halfWidth, halfHeight, 0f);
            Vector3 p3 = origin + new Vector3(halfWidth, -halfHeight, 0f);
            Vector3 p4 = origin + new Vector3(-halfWidth, -halfHeight, 0f);

            Handles.color = Color.yellow;
            Handles.DrawLines(new[] {p1, p2, p2, p3, p3, p4, p4, p1});
        }
        #endif

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        #region IDamageable Implementations

        /// <summary>
        /// Changes the current <see cref="IPlayerState"/> to the death state and plays associated SFX.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnPlayerStateChanged"/> event. </remarks>
        public void Die() {
            this._soundsController.OnHenDeath();
            this.ChangeState(State.Dead);
        }

        // TODO: If the health bar is implemented, heroes will damage the player through IDamageable
        public void TakeDamage(int amount) {}

        #endregion

        #region ITrapDamageable Implementations

        // TODO: Do we want the player to have a health bar?
        public int GetHealth() {
            return -1;
        }

        // TODO: If the player health is implemented, traps will damage the player through ITrapDamageable
        public void TakeDamage(int amount, Trap trap) {}

        public void ApplySpeedPenalty(float penalty) {}

        public void ResetSpeedPenalty() {}

        public void ThrowActorForward(float throwForce) {}

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
            this._animator.SetBool(_groundedKey, this.IsGrounded);

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
            this._state?.OnExit(nextState);
            this._state = nextState;
            this._state?.OnEnter(prevState);
            this._stateChangedThisFrame = true;
            this.OnPlayerStateChanged?.Invoke(this._state, this.transform.position);
        }

        public void PlayJumpSound() {
            this._soundsController.OnHenJump();
        }

        /// <summary>
        /// Resets vertical velocity to 0. Needed when jumping after started falling when vertical velocity is already negative.
        /// </summary>
        public void StopFalling() {
            this.RBody.velocity = new Vector2(this.RBody.velocity.x, 0f);
        }

        #endregion

        #region Pawn Inherited Methods

        /// <inheritdoc cref="Pawn.ChangeTrap"/>
        /// <remarks>
        /// Delegates trap selection execution to the <see cref="TrapController"/>.
        /// Invokes the <see cref="OnSelectedTrapChanged"/> event.
        /// </remarks>
        public override void ChangeTrap(Trap trap) {
            // Delegate setting trap to TrapController for better encapsulation and efficiency
            this._trapController.ChangeTrap(trap);
            this.OnSelectedTrapChanged?.Invoke(trap);
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

        public override void FreezePosition() {
            base.FreezePosition();
            this._isFrozen = true;
        }

        #endregion

        /// <summary>
        /// Sets up all listeners and delegates to operate the <see cref="PlayerCharacter"/>.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnSelectedTrapChanged"/> event. </remarks>
        /// <param name="onHenWon"> An event to notify listeners when a level has been completed. </param>
        /// <param name="getControllerInput"> A delegate callback used to fetch input from the controller that
        /// possesses this player entity. </param>
        public void Initialize(UnityEvent<string> onHenWon, InputDelegate<float, bool, bool> getControllerInput) {
            this._gatherControllerInput = getControllerInput;

            onHenWon.AddListener(this.GameOver);

            this.OnSelectedTrapChanged?.Invoke(this._trapController.CurrentTrap);
        }

        /// <summary>
        /// Selects the specified trap for placement evaluation in the level.
        /// </summary>
        /// <param name="selectedTrap"> The new trap to be selected for placement. </param>
        public void SetTrap(Trap selectedTrap) {
            var command = new SetTrapCommand(this, selectedTrap);
            this.ExecuteCommand(command);
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
        /// Retrieves the resource recipe of the current selected trap through the <see cref="TrapController"/>.
        /// </summary>
        /// <returns> The cost of the current selected trap. </returns>
        public Dictionary<ResourceType, int> GetTrapCost() {
            return this._trapController.GetCurrentTrapCost();
        }

        #endregion

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

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        #region Input

        /// <summary>
        /// Executes the <see cref="DeployCommand"/>.
        /// </summary>
        public void InvokeDeployTrap() {
            this.ExecuteCommand(this._deployCommand);
        }

        /// <summary>
        /// Reads input for move and jump. Puts read values in the <c>out</c> variables.
        /// </summary>
        /// <param name="moveInput"> Move input is saved into this variable. </param>
        /// <param name="jumpPressed"> Jump Button pressed is saved into this variable. </param>
        /// <param name="jumpPressedThisFrame"> Jump Button pressed this frame is saved into this variable. </param>
        private void GatherInput(out float moveInput, out bool jumpPressed, out bool jumpPressedThisFrame) {
            try {
                this._gatherControllerInput(out moveInput, out jumpPressed, out jumpPressedThisFrame);
            } catch (Exception error) {
                Debug.LogError(error);

                moveInput = 0f;
                jumpPressed = jumpPressedThisFrame = false;
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._direction = moveInput > 0 ? 1 : -1;
            }
        }

        #endregion

        /// <summary>
        /// Adjusts the <see cref="Animator"/> speed and direction values.
        /// </summary>
        /// <param name="inputDirection"> The vector x-value associated with the player's current movement. </param>
        private void SetAnimatorValues(float inputDirection) {
            this._animator.SetFloat(_speedKey, Mathf.Abs(inputDirection));
            this._animator.SetFloat(_directionKey, this._direction);
        }

        /// <summary>
        /// Sets the player entity's state to reflect the end of the level.
        /// </summary>
        /// <remarks> Subscribed to the OnHenWon core game system event. </remarks>
        private void GameOver(string _) {
            this.ChangeState(State.GameOver);
        }

        /// <summary>
        /// Casts a box below the player to check whether player is grounded. Updates the <see cref="IsGrounded"/> variable.
        /// </summary>
        /// <remarks> Invokes <see cref="OnPlayerGrounded"/>. </remarks>
        private void UpdateGrounded() {
            Collider2D hit = Physics2D.OverlapBox(
                (Vector2)this.transform.position + this.GroundedCheckBoxOffset,
                this.GroundedCheckBoxSize, 0f, this.GroundedLayerMask
            );

            bool grounded = hit != null;
            bool becameNotGrounded = this.IsGrounded && !grounded;
            bool becameGrounded = !this.IsGrounded && grounded;

            if (becameNotGrounded) {
                if (this._coyoteTimeCoroutine != null) {
                    return;
                }

                this._coyoteTimeCoroutine = this.CoyoteTimeCoroutine();
                this.StartCoroutine(this._coyoteTimeCoroutine);
                return;
            }

            if (becameGrounded) {
                this.OnPlayerGrounded?.Invoke();
                if (this._coyoteTimeCoroutine != null) {
                    this.StopCoroutine(this._coyoteTimeCoroutine);
                    this._coyoteTimeCoroutine = null;
                }
            }

            this.SetGroundedStatus(grounded);
        }

        /// <summary>
        /// Checks if the player is falling. Updates <see cref="IsFalling"/>.
        /// </summary>
        /// <remarks> If player started falling this frame, invokes <see cref="OnPlayerFalling"/> </remarks>
        private void UpdateFalling() {
            bool falling = this.RBody.velocity.y < -0.1f;
            bool becameFalling = !this.IsFalling && falling;

            if (becameFalling) {
                this.OnPlayerFalling?.Invoke();
            }

            this.IsFalling = falling;
        }

        /// <summary>
        /// A coroutine that holds <see cref="IsGrounded"/> as <c>true</c> during coyote timer.
        /// </summary>
        private IEnumerator CoyoteTimeCoroutine() {
            yield return new WaitForSeconds(CoyoteTimeDuration);
            this.SetGroundedStatus(false);
        }

        #endregion
    }
}
