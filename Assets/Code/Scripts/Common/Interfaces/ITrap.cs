//*******************************************************************************************
// ITrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Common.Interfaces {
    /// <summary>
    /// Interfaces any trap that is deployed by the player to damage heroes. Exposes trap
    /// status checkers to other systems, such as when it's build is completed.
    /// </summary>
    public interface ITrap {
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Retrieves the trap's build completion status.
        /// </summary>
        /// <returns> If the trap building process has been completed. </returns>
        public bool IsTrapReady();
        
        #endregion
    }
}
