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

        private void OnTriggerEnter2D(Collider2D other) {
            print("Triggered!");
            if (!other.CompareTag("Player"))
                return;

            print("Triggered by player!");
            this.Detonate();
        }
    }
}
