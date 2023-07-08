using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    public class TestTrap1 : Trap
    {
        public override Vector3 GetLeftSpawnPoint(Vector3 origin)
        {
            return origin + _leftSpawnOffset;
        }
        
        public override Vector3 GetRightSpawnPoint(Vector3 origin)
        {
            return origin + _rightSpawnOffset;
        }

        public override void Detonate()
        {
            
        }
    }
}
