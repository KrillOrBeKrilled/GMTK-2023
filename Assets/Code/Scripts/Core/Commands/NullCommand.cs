//*******************************************************************************************
// NullCommand
//*******************************************************************************************
using KrillOrBeKrilled.Core.Commands.Interfaces;

namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// A Null Object Command that implements <see cref="ICommand"/> to do nothing.
    /// </summary>
    /// <remarks>
    /// Included in case we want to set up buttons that do nothing
    /// (e.g. switching keybindings).
    /// </remarks>
    public class NullCommand : ICommand {
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        public void Execute() {}
        
        #endregion
        
    }
}
