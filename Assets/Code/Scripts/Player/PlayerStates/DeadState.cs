//*******************************************************************************************
// DeadState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player death.
    /// </summary>
    public class DeadState : IPlayerState {

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Player died
        }

        public void OnEnter(IPlayerState prevState) {

        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
