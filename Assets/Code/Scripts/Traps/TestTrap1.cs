using UnityEngine;

namespace Traps
{
    public class TestTrap1 : Trap
    {
        public override void AdjustSpawnPoint()
        {

        }

        protected  override void TriggerTrap(Hero hero)
        {
            print("Detonating!");
        }
    }
}
