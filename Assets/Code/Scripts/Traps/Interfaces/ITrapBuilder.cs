//*******************************************************************************************
// ITrapBuilder
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps.Interfaces {
    /// <summary>
    /// Interfaces any actor that has the ability to build traps.
    /// </summary>
    public interface ITrapBuilder {
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Determines if the actor can build the trap at the current time.
        /// </summary>
        /// <returns> If the actor can currently build the trap. </returns>
        /// <remarks> Availability to build the trap is heavily dependent on the actor state. </remarks>
        public bool CanBuildTrap();

        /// <summary>
        /// Adjusts animations and trap building availability depending on whether the actor has touched the
        /// ground or not.
        /// </summary>
        /// <param name="isGrounded"> If the actor is currently touching the ground. </param>
        public void SetGroundedStatus(bool isGrounded);
        
        #endregion
    }
}
