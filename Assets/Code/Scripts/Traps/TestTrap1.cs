using UnityEngine;

namespace Traps
{
    public class TestTrap1 : Trap
    {
        public override void AdjustSpawnPoint()
        {

        }

        protected  override void OnEnteredTrap(Hero hero)
        {
            print("Triggering Trap");
        }

        protected override void OnExitedTrap(Hero hero) {
            print("Left Trap");
        }
    }
}
