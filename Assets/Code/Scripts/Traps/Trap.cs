using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Transform> _gridPoints;

        public abstract void AdjustSpawnPoint();
        protected abstract void OnEnteredTrap(Hero hero);
        protected abstract void OnExitedTrap(Hero hero);

        private void OnTriggerEnter2D(Collider2D other) {
            this.OnEnteredTrap(other.GetComponent<Hero>());
        }

        private void OnTriggerExit2D(Collider2D other) {
            this.OnExitedTrap(other.GetComponent<Hero>());
        }
    }
}