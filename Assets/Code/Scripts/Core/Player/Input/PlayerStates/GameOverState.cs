//*******************************************************************************************
// GameOverState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player death.
    /// </summary>
    public class GameOverState : IPlayerState {

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void Act(PlayerController playerController, float direction) {
            // The game is over! Maybe the player shouldn't do anything anymore and some UI opens or something
        }

        public float GetMovementSpeed() {
            return 0f;
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the game ends...what should happen? music? visual animations?
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the game ends...what should happen? music? visual animations?
        }

        public bool ShouldExit() {
            // No exit conditions for this state
            return false;
        }

        #endregion
    }
}
