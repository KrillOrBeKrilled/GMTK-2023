//*******************************************************************************************
// JumpCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for jumping.
    /// </summary>
    public class JumpCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// The move input for this command.
        private readonly float _moveInput;

        /// The jump force for this command.
        private readonly float _jumpForce;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void Execute() {
            this._controlledObject.Move(this._moveInput);
            this._controlledObject.Jump(this._jumpForce);
        }

        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this jump command. </param>
        /// <param name="moveInput"> The direction to move the player <see cref="Pawn"/> on command execution. </param>
        /// <param name="jumpForce"> The jump force. </param>
        /// ;
        public JumpCommand(Pawn controlledObject, float moveInput, float jumpForce) {
            this._controlledObject = controlledObject;
            this._moveInput = moveInput;
            this._jumpForce = jumpForce;
        }

        #endregion
    }
}
