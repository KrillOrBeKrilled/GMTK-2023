//*******************************************************************************************
// MoveCommand
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for movement.
    /// </summary>
    public class MoveCommand : ICommand {
        // Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;
        
        // Direction for the player Pawn to move towards on execution of this command.
        private readonly float _inputDirection;

        /// <summary> Constructor to set references required for this command to act on a <see cref="Pawn"/>. </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this move command. </param>
        /// <param name="inputDirection"> The direction to move the player <see cref="Pawn"/> on command execution. </param>
        public MoveCommand(Pawn controlledObject, float inputDirection) {
            this._controlledObject = controlledObject;
            this._inputDirection = inputDirection;
        }
        
        public float GetDirection() {
            return this._inputDirection;
        }

        public void Execute() {
            this._controlledObject.Move(this._inputDirection);
        }
    }
}
