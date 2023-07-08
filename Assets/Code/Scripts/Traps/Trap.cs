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
        protected abstract void Detonate(Collider2D target);

        private void OnTriggerEnter2D(Collider2D other) {
            this.Detonate(other);
        }
    }
}
