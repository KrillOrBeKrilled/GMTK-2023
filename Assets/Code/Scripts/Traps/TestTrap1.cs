using KrillOrBeKrilled.Common;

//*******************************************************************************************
// TestTrap1
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// An experimental subclass of <see cref="Trap"/> used to test various trap
    /// deployment and detonation scenarios. 
    /// </summary>
    public class TestTrap1 : Trap {
        protected override void SetUpTrap() {}

        protected override void DetonateTrap() {}

        protected  override void OnEnteredTrap(IDamageable actor) {}

        protected override void OnExitedTrap(IDamageable actor) {}
    }
}
