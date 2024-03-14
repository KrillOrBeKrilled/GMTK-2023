//*******************************************************************************************
// AttackCommand
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for attacking.
    /// </summary>
    public class AttackCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this deploy command. </param>
        public AttackCommand(Pawn controlledObject) {
            this._controlledObject = controlledObject;
        }

        public void Execute() {
            this._controlledObject.Attack();
        }
        
        #endregion
    }
}