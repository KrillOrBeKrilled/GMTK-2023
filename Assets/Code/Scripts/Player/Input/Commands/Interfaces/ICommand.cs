//*******************************************************************************************
// ICommand
//*******************************************************************************************
namespace Player {
    /// <summary>
    /// General interface for a command that encapsulates a specific player input action.
    /// </summary>
    /// <remarks>  Includes getters for retrieving data and a general execute method to
    /// execute the action associated with the specific type of command. </remarks>
    public interface ICommand {
        /// <summary> Executes the input action associated with this command. </summary>
        public void Execute();

        /// <summary> Retrieves the current direction of the player. </summary>
        /// <returns> The direction the player is facing on the x-axis. </returns>
        public float GetDirection() {
            return 0f;
        }

        /// <summary> Retrieves the index of the current selected trap. </summary>
        /// <returns> The index of the selected trap. </returns>
        public int GetTrapIndex() {
            return 0;
        }
    }
}
