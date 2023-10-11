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

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="IdleCommand"/>. </description>
        public void Act(PlayerController playerController, float moveInput, bool jumpTriggered) {
            // Check if need to change state
            if (jumpTriggered) {
                playerController.ChangeState(PlayerController.State.Jumping);
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                playerController.ChangeState(PlayerController.State.Moving);
                return;
            }

            // Create command and execute it
            var command = new IdleCommand(playerController);
            playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player walks...what should happen? music? visual animations? Does it matter from which
            // state?
            Debug.Log("Idle");
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops idling...what should happen? music? visual animations? Does
            // it matter to which state?
        }

        #endregion
    }
}
