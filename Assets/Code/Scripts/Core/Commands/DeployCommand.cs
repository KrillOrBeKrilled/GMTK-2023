//*******************************************************************************************
// DeployCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for trap
    /// deployment.
    /// </summary>
    public class DeployCommand : ICommand {
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
        public DeployCommand(Pawn controlledObject) {
            this._controlledObject = controlledObject;
        }

        public void Execute() {
            this._controlledObject.DeployTrap();
        }
        
        #endregion
    }
}
