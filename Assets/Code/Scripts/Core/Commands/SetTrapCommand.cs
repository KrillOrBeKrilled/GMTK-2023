//*******************************************************************************************
// SetTrapCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;
using KrillOrBeKrilled.Traps;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for equipping
    /// traps.
    /// </summary>
    public class SetTrapCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// Index of the trap for the player Pawn to equip on execution of this command.
        private readonly Trap _trap;

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        public void Execute() {
            this._controlledObject.ChangeTrap(this._trap);
        }
        
        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this set trap command. </param>
        /// <param name="trap"> The trap to equip on command execution. </param>
        public SetTrapCommand(Pawn controlledObject, Trap trap) {
            this._controlledObject = controlledObject;
            this._trap = trap;
        }
        
        #endregion
    }
}
