using KrillOrBeKrilled.Core.Commands;

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
        public void Act(PlayerController playerController, float direction) {
            // Create command and execute it
            var command = new IdleCommand(playerController);
            playerController.ExecuteCommand(command);
        }

        public float GetMovementSpeed() {
            return 0f;
        }
        
        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player walks...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops idling...what should happen? music? visual animations? Does
            // it matter to which state?
        }
        
        #endregion
    }
}
