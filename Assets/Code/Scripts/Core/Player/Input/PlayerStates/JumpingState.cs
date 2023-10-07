using Codice.Client.BaseCommands;
using KrillOrBeKrilled.Core.Commands;
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
        private readonly float _stateSpeed;
        private float _jumpStartTime;
        private readonly float _jumpForceMaxDuration;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        /// <param name="stateSpeed"> The movement speed of the player to be executed in this state. </param>
        /// <param name="jumpForceMaxDuration"> The</param>
        public JumpingState(float stateSpeed, float jumpForceMaxDuration) {
            this._stateSpeed = stateSpeed;
            this._jumpForceMaxDuration = jumpForceMaxDuration;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="JumpCommand"/>. </description>
        public void Act(PlayerController playerController, float moveInput) {
            // Create command and execute it
            var command = new JumpCommand(playerController, moveInput);
            playerController.ExecuteCommand(command);
        }

        public float GetMovementSpeed() {
            return this._stateSpeed;
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
            this._jumpStartTime = Time.time;
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }

        public bool ShouldExit() {
            return Time.time > this._jumpStartTime + this._jumpForceMaxDuration;
        }

        #endregion
    }
}
