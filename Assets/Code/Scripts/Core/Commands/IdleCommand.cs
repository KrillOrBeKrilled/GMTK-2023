//*******************************************************************************************
// IdleCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for standing idle.
    /// </summary>
    public class IdleCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        public void Execute() {
            this._controlledObject.StandIdle();
        }
        
        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this idle command. </param>
        public IdleCommand(Pawn controlledObject) {
            this._controlledObject = controlledObject;
        }
        
        #endregion
    }
}
