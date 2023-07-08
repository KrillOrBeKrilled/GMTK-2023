using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Transform> _gridPoints;

        public abstract void AdjustSpawnPoint();
        protected abstract void TriggerTrap(Hero hero);

        private void OnTriggerEnter2D(Collider2D other) {
            this.TriggerTrap(other.GetComponent<Hero>());
        }
    }
}
