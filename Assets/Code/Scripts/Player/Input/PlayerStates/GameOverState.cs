using KrillOrBeKrilled.Player.Commands;

//*******************************************************************************************
// GameOverState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.Input.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player death.
    /// </summary>
    public class GameOverState : IPlayerState {
        private readonly PlayerController _playerController;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public GameOverState(PlayerController playerController) {
            this._playerController = playerController;
        }

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // The game is over!
            this._playerController.DisablePlayerInput();
            FreezeCommand command = new FreezeCommand(this._playerController);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {

        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
