using KrillOrBeKrilled.Core.Commands;
using UnityEngine;

//*******************************************************************************************
// IdleState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player idle state.
    /// </summary>
    public class IdleState : IPlayerState {
        private readonly PlayerController _playerController;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public IdleState(PlayerController playerController) {
            this._playerController = playerController;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="IdleCommand"/>. </description>
        public void Act(float moveInput, bool jumpTriggered) {
            // Check if need to change state
            if (jumpTriggered && this._playerController.IsGrounded) {
                this._playerController.ChangeState(PlayerController.State.Jumping);
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._playerController.ChangeState(PlayerController.State.Moving);
                return;
            }

            // Create command and execute it
            var command = new IdleCommand(this._playerController);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            Debug.Log("Idle");
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
