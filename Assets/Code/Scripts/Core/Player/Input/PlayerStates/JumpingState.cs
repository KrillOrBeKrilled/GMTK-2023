using Codice.Client.BaseCommands;
using KrillOrBeKrilled.Core.Commands;
using System.Collections;
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
        private float _jumpStartTime;
        private readonly float _jumpForceMaxDuration;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        /// <param name="jumpForceMaxDuration"> The</param>
        public JumpingState(float jumpForceMaxDuration) {
            this._jumpForceMaxDuration = jumpForceMaxDuration;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="JumpCommand"/>. </description>
        /// <param name="playerController"></param>
        /// <param name="moveInput"></param>
        /// <param name="jumpTriggered"></param>
        public void Act(PlayerController playerController, float moveInput, bool jumpTriggered) {
            // Check if need to change state
            bool noMovement = Mathf.Approximately(moveInput, 0f);
            if (!jumpTriggered && noMovement) {
                playerController.ChangeState(PlayerController.State.Idle);
                return;
            }

            if (!jumpTriggered) {
                playerController.ChangeState(PlayerController.State.Moving);
            }

            // Create command and execute it
            var command = new JumpCommand(playerController, moveInput);
            playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
            this._jumpStartTime = Time.time;
            Debug.Log("Jump");
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }

        #endregion
    }
}
