//*******************************************************************************************
// IPlayerState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Used to declutter the <see cref="PlayerController"/> class, encapsulating the
    /// player's behaviour in each state to better reason about correctness and
    /// pinpoint bugs easily, plus specialize behaviours to specific states.
    /// </summary>
    public interface IPlayerState {

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Adjust the player movement speed and actions depending on the current state of the player.
        /// <list type="bullet">
        /// <item> Idle: Player stands still, can transition to any other state in this state. </item>
        /// <item> Moving: Player moves in this state in a given direction along the x-axis. </item>
        /// <item> GameOver: Player has perished, thus ending the game. </item>
        /// </list>
        /// </summary>
        /// <param name="moveInput"> The move input. </param>
        /// <param name="jumpTriggered">A bool indicating if the jump input triggered</param>
        public void Act(float moveInput, bool jumpTriggered);

        /// <summary>
        /// Executes animations, SFX, logic, and so forth associated with the entry into this player state.
        /// </summary>
        /// <param name="prevState"> The <see cref="IPlayerState"/> that was exited to enter this state. </param>
        public void OnEnter(IPlayerState prevState);

        /// <summary>
        /// Executes animations, SFX, logic, and so forth associated with the exit out of this player state.
        /// </summary>
        /// <param name="newState"> The <see cref="IPlayerState"/> that will be entered from this state. </param>
        public void OnExit(IPlayerState newState);

        #endregion
    }
}
