using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Vector3Int> _leftGridPoints, _rightGridPoints;
        [SerializeField] protected int _validationScore;

        public List<Vector3Int> GetLeftGridPoints()
        {
            return _leftGridPoints;
        }
        
        public List<Vector3Int> GetRightGridPoints()
        {
            return _rightGridPoints;
        }
        
        public bool IsValidScore(int score)
        {
            return score >= _validationScore;
        }
        
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
