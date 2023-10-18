//*******************************************************************************************
// GlideCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Implements <see cref="ICommand"/> to execute the player action for gliding.
    /// </summary>
    public class GlideCommand : ICommand {
        /// Reference to the player Pawn to control.
        private readonly Pawn _controlledObject;

        /// The move input for this command.
        private readonly float _moveInput;

        private readonly float _glideSpeedMultiplier;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void Execute() {
            this._controlledObject.Glide(this._moveInput, this._glideSpeedMultiplier);
        }

        /// <summary>
        /// Constructor to set references required for this command to act on a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="controlledObject"> The player <see cref="Pawn"/> associated with this jump command. </param>
        /// <param name="moveInput"> The direction to move the player <see cref="Pawn"/> on command execution. </param>
        /// <param name="glideSpeedMultiplier"> The multiplier for the glide speed. </param>
        /// ;
        public GlideCommand(Pawn controlledObject, float moveInput, float glideSpeedMultiplier) {
            this._controlledObject = controlledObject;
            this._moveInput = moveInput;
            this._glideSpeedMultiplier = glideSpeedMultiplier;
        }

        #endregion
    }
}
