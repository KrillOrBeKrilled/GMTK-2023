using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    public class TestTrap1 : Trap
    {
        public override Vector3 GetLeftSpawnPoint(Vector3 origin)
        {
            return origin + LeftSpawnOffset;
        }
        
        public override Vector3 GetRightSpawnPoint(Vector3 origin)
        {
            return origin + RightSpawnOffset;
        }
        
        protected  override void OnEnteredTrap(Hero hero) {
        }

        protected override void OnExitedTrap(Hero hero) {
        }
    }
}
