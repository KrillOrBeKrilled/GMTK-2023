//*******************************************************************************************
// NullCommand
//*******************************************************************************************
namespace Player {
    /// <summary>
    /// A Null Object Command that implements <see cref="ICommand"/> to do nothing.
    /// </summary>
    /// <remarks> Included in case we want to set up buttons that do nothing
    /// (e.g. switching keybindings) </remarks>
    public class NullCommand : ICommand {
        public void Execute() {

        }
    }
}
