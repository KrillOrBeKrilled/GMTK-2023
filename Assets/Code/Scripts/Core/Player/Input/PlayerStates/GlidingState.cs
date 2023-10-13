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
    public class GlidingState : IPlayerState {
        private readonly PlayerController _playerController;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        public GlidingState(PlayerController playerController) {
            this._playerController = playerController;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="GlideCommand"/> </description>
        /// <param name="moveInput"></param>
        /// <param name="jumpTriggered"></param>
        public void Act(float moveInput, bool jumpTriggered) {
            // Check if need to change state
            if (this._playerController.IsGrounded || !jumpTriggered) {
                bool isMoving = !Mathf.Approximately(moveInput, 0f);
                this._playerController.ChangeState(isMoving ? State.Moving : State.Idle);
                return;
            }

            // Create command and execute it
            var command = new GlideCommand(this._playerController, moveInput);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            Debug.Log("Glide");
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
