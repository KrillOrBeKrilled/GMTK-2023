using KrillOrBeKrilled.Common.Interfaces;

//*******************************************************************************************
// TestTrap1
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// An experimental subclass of <see cref="Trap"/> used to test various trap
    /// deployment and detonation scenarios. 
    /// </summary>
    public class TestTrap1 : Trap {
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        protected override void DetonateTrap() {}

        protected  override void OnEnteredTrap(IDamageable actor) {}

        protected override void OnExitedTrap(IDamageable actor) {}
        
        protected override void SetUpTrap() {}
        
        #endregion
    }
}
