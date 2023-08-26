using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TrapOverlap : MonoBehaviour
    {
        private Collider2D _currentCollision;

        public Collider2D GetCollisionData()
        {
            return this._currentCollision;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Trap"))
            {
                this._currentCollision = other;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Trap"))
            {
                this._currentCollision = null;
            }
        }
    }

}
