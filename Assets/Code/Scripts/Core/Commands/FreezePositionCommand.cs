//*******************************************************************************************
// FreezeCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to freeze the player position.
    /// </summary>
    public class FreezeCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// <summary> Constructor to set references required for this command to act on a <see cref="Pawn"/>. </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this deploy command. </param>
        public FreezeCommand(Pawn controlledObject) {
            this._controlledObject = controlledObject;
        }

        public void Execute() {
            this._controlledObject.FreezePosition();
        }
    }
}
