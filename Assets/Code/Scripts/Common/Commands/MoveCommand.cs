//*******************************************************************************************
// MoveCommand
//*******************************************************************************************
namespace KrillOrBeKrilled.Common.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for movement.
    /// </summary>
    public class MoveCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// Direction for the player Pawn to move towards on execution of this command.
        private readonly float _moveInput;

        /// <summary> Constructor to set references required for this command to act on a <see cref="Pawn"/>. </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this move command. </param>
        /// <param name="moveInput"> The moveInput value of the player <see cref="Pawn"/> on command execution. </param>
        public MoveCommand(Pawn controlledObject, float moveInput) {
            this._controlledObject = controlledObject;
            this._moveInput = moveInput;
        }

        public float GetMoveInput() {
            return this._moveInput;
        }

        public void Execute() {
            this._controlledObject.Move(this._moveInput);
        }
    }
}
