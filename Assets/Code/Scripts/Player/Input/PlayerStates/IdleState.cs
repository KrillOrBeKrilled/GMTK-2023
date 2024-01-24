using KrillOrBeKrilled.Player.Commands;
using State = KrillOrBeKrilled.Player.Input.PlayerController.State;
using UnityEngine;

//*******************************************************************************************
// IdleState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.Input.PlayerStates {
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
        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            if (jumpPressedThisFrame) {
                State nextState = this._playerController.IsGrounded ? State.Jumping : State.Gliding;
                this._playerController.ChangeState(nextState);
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._playerController.ChangeState(State.Moving);
                return;
            }

            // Create command and execute it
            var command = new IdleCommand(this._playerController);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {

        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
