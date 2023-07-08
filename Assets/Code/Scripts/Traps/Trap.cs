using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Transform> _gridPoints;

        public abstract void AdjustSpawnPoint();
        public abstract void Detonate();
    }
}