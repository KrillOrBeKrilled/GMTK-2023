using KrillOrBeKrilled.Core.Commands;

//*******************************************************************************************
// MovingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player movement state.
    /// </summary>
    public class MovingState : IPlayerState {
        // TODO: Adjust multiplier values here
        private readonly float _stateSpeed;

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        /// <param name="stateSpeed"> The movement speed of the player to be executed in this state. </param>
        public MovingState(float stateSpeed) {
            this._stateSpeed = stateSpeed;
        }

        /// <inheritdoc cref="IPlayerState.Act"/>
        /// <description> Executes the <see cref="MoveCommand"/>. </description>
        public void Act(PlayerController playerController, float moveInput) {
            // Create command and execute it
            var command = new MoveCommand(playerController, moveInput);
            playerController.ExecuteCommand(command);
        }

        public float GetMovementSpeed() {
            return this._stateSpeed;
        }

        public void OnEnter(IPlayerState prevState) {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public void OnExit(IPlayerState newState) {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }
        
        #endregion
    }
}
