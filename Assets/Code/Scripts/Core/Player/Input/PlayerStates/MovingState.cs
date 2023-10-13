using KrillOrBeKrilled.Core.Commands;
using UnityEngine;
using State = KrillOrBeKrilled.Core.Player.PlayerController.State;

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
        private readonly PlayerController _playerController;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public MovingState(PlayerController playerController) {
            this._playerController = playerController;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="MoveCommand"/>.</description>
        public void Act(float moveInput, bool jumpTriggered) {
            // Check if need to change state
            if (jumpTriggered) {
                State nextState = this._playerController.IsGrounded ? State.Jumping : State.Gliding;
                this._playerController.ChangeState(nextState);
                return;
            }

            if (Mathf.Approximately(moveInput, 0f)) {
                this._playerController.ChangeState(State.Idle);
                return;
            }

            // Create command and execute it
            var command = new MoveCommand(this._playerController, moveInput);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            Debug.Log("Move");
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
