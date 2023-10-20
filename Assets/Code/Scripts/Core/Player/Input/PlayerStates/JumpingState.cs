using KrillOrBeKrilled.Core.Commands;
using State = KrillOrBeKrilled.Core.Player.PlayerController.State;
using UnityEngine;

//*******************************************************************************************
// JumpingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player jumping state.
    /// </summary>
    public class JumpingState : IPlayerState {
        private readonly PlayerController _playerController;

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
        public JumpingState(PlayerController playerController) {
            this._playerController = playerController;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="JumpCommand"/>. </description>
        /// <param name="moveInput"></param>
        /// <param name="jumpPressed"></param>
        /// <param name="jumpPressedThisFrame"></param>
        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            bool jumpTimeExceeded = Time.time > this._jumpStartTime + JumpForceMaxDuration;
            if ((!this._playerController.IsGrounded && this._playerController.IsFalling) || jumpTimeExceeded) {
                this._playerController.ChangeState(State.Gliding);
                return;
            }

            bool noMovement = Mathf.Approximately(moveInput, 0f);
            if (!jumpPressed) {
                State nextState = noMovement ? State.Idle : State.Moving;
                this._playerController.ChangeState(nextState);
                return;
            }

            bool shouldPlaySound = Time.time > this._jumpSoundPlayTime + SoundInterval;
            if (shouldPlaySound) {
                this._jumpSoundPlayTime = Time.time;
                this._playerController.PlayJumpSound();
            }

            // Create command and execute it
            float jumpForce = jumpPressedThisFrame ? JumpStartForce : JumpHoldForce;
            var command = new JumpCommand(this._playerController, moveInput, jumpForce);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            this._jumpStartTime = Time.time;
            this._jumpSoundPlayTime = Time.time;
            this._playerController.PlayJumpSound();
            this._playerController.StopFalling();
            this._playerController.SetGroundedStatus(false);
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
