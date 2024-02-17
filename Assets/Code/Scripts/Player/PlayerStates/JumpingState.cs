using KrillOrBeKrilled.Player.Commands;
using State = KrillOrBeKrilled.Player.PlayerCharacter.State;
using UnityEngine;

//*******************************************************************************************
// JumpingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player jumping state.
    /// </summary>
    public class JumpingState : IPlayerState {
        private readonly PlayerCharacter _player;

        private const float JumpStartForce = 15f;
        private const float JumpHoldForce = 1f;
        private const float SoundInterval = 0.15f;
        private const float JumpForceMaxDuration = 0.35f;

        private float _jumpStartTime;
        private float _jumpSoundPlayTime;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        public JumpingState(PlayerCharacter player) {
            this._player = player;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="JumpCommand"/>. </description>
        /// <param name="moveInput"></param>
        /// <param name="jumpPressed"></param>
        /// <param name="jumpPressedThisFrame"></param>
        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            bool jumpTimeExceeded = Time.time > this._jumpStartTime + JumpForceMaxDuration;
            if ((!this._player.IsGrounded && this._player.IsFalling) || jumpTimeExceeded) {
                this._player.ChangeState(State.Gliding);
                return;
            }

            bool noMovement = Mathf.Approximately(moveInput, 0f);
            if (!jumpPressed) {
                State nextState = noMovement ? State.Idle : State.Moving;
                this._player.ChangeState(nextState);
                return;
            }

            bool shouldPlaySound = Time.time > this._jumpSoundPlayTime + SoundInterval;
            if (shouldPlaySound) {
                this._jumpSoundPlayTime = Time.time;
                this._player.PlayJumpSound();
            }

            // Create command and execute it
            float jumpForce = jumpPressedThisFrame ? JumpStartForce : JumpHoldForce;
            var command = new JumpCommand(this._player, moveInput, jumpForce);
            this._player.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            this._jumpStartTime = Time.time;
            this._jumpSoundPlayTime = Time.time;
            this._player.PlayJumpSound();
            this._player.StopFalling();
            this._player.SetGroundedStatus(false);
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
