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

        /// Direction for the player Pawn to move towards on execution of this command.
        private readonly float _inputDirection;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void Execute() {
            this._controlledObject.Move(this._inputDirection);
            this._controlledObject.Jump();
        }

        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this jump command. </param>
        /// /// &lt;param name="inputDirection"&gt; The direction to move the player &lt;see cref="Pawn"/&gt; on command execution. &lt;/param&gt;
        public JumpCommand(Pawn controlledObject, float inputDirection) {
            this._controlledObject = controlledObject;
            this._inputDirection = inputDirection;
        }

        #endregion
    }
}
