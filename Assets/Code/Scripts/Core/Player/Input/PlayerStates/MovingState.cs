using KrillOrBeKrilled.Core.Commands;
using UnityEngine;

//*******************************************************************************************
// MovingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player movement state.
    /// </summary>
    public class MovingState : IPlayerState {
        // TODO: Adjust multiplier values here

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="MoveCommand"/>.</description>
        public void Act(PlayerController playerController, float moveInput, bool jumpTriggered) {
            // Check if need to change state
            if (jumpTriggered) {
                playerController.ChangeState(PlayerController.State.Jumping);
                return;
            }

            if (Mathf.Approximately(moveInput, 0f)) {
                playerController.ChangeState(PlayerController.State.Idle);
                return;
            }

            // Create command and execute it
            var command = new MoveCommand(playerController, moveInput);
            playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
            Debug.Log("Move");
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }

        #endregion
    }
}
