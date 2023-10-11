//*******************************************************************************************
// GameOverState
//*******************************************************************************************
using UnityEngine;

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

        public void Act(float moveInput, bool jumpTriggered) {
            // The game is over! Maybe the player shouldn't do anything anymore and some UI opens or something
        }

        public void OnEnter(IPlayerState prevState) {
            Debug.Log("Game Over");
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
