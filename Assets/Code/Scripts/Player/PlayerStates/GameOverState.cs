using KrillOrBeKrilled.Player.Commands;

//*******************************************************************************************
// GameOverState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player death.
    /// </summary>
    public class GameOverState : IPlayerState {
        private readonly PlayerCharacter _player;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public GameOverState(PlayerCharacter player) {
            this._player = player;
        }

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // The game is over!
            
            // TODO: Do this in another way
            // this._player.DisablePlayerInput();
            
            FreezeCommand command = new FreezeCommand(this._player);
            this._player.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {

        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
