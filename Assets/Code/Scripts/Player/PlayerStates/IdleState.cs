using KrillOrBeKrilled.Player.Commands;
using State = KrillOrBeKrilled.Player.PlayerCharacter.State;
using UnityEngine;

//*******************************************************************************************
// IdleState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player idle state.
    /// </summary>
    public class IdleState : IPlayerState {
        private readonly PlayerCharacter _player;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public IdleState(PlayerCharacter player) {
            this._player = player;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="IdleCommand"/>. </description>
        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            if (jumpPressedThisFrame) {
                State nextState = this._player.IsGrounded ? State.Jumping : State.Gliding;
                this._player.ChangeState(nextState);
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._player.ChangeState(State.Moving);
                return;
            }

            // Create command and execute it
            var command = new IdleCommand(this._player);
            this._player.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {

        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
