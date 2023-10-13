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

        private const float SoundInterval = 0.25f;
        private const float JumpForceMaxDuration = 0.6f;
        private const float FinalJumpForceReductionPercent = 0.8f;

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
        /// <param name="jumpTriggered"></param>
        public void Act(float moveInput, bool jumpTriggered) {
            // Check if need to change state
            bool jumpTimeExceeded = Time.time > this._jumpStartTime + JumpForceMaxDuration;
            if (this._playerController.IsFalling || jumpTimeExceeded) {
                this._playerController.ChangeState(State.Gliding);
                return;
            }

            bool noMovement = Mathf.Approximately(moveInput, 0f);
            if (!jumpTriggered) {
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
            float timePassedPercentage = (Time.time - this._jumpStartTime) / JumpForceMaxDuration;
            float jumpForceMultiplier = 1f - timePassedPercentage * FinalJumpForceReductionPercent;
            var command = new JumpCommand(this._playerController, moveInput, jumpForceMultiplier);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            Debug.Log("Jump");
            this._jumpStartTime = Time.time;
            this._jumpSoundPlayTime = Time.time;
            this._playerController.OnJumpStart();
            this._playerController.PlayJumpSound();
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
