//*******************************************************************************************
// JumpCommand
//*******************************************************************************************
namespace KrillOrBeKrilled.Common.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for jumping.
    /// </summary>
    public class JumpCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// <summary> Constructor to set references required for this command to act on a <see cref="Pawn"/>. </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this jump command. </param>
        public JumpCommand(Pawn controlledObject) {
            this._controlledObject = controlledObject;
        }

        public void Execute() {
            this._controlledObject.Jump();
        }
    }
}
