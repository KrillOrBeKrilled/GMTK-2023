using System;
using UnityEngine;

//*******************************************************************************************
// ResourcePickup
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A class to be attached to each resource pickup game object.
    /// </summary>
    public class ResourcePickup : MonoBehaviour {
        [SerializeField] private ResourcePickupData data;
        private Rigidbody2D _rigidbody2D;
        
        // Currently, using a static event to keep Managers depending on Traps. (To be refactored later)
        public static event Action<ResourceType, int> OnResourceCollected;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Awake() {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Player")) {
                OnResourceCollected?.Invoke(data.resourceType, data.quantity);
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Ground")) {
                ReduceMovement();
            }
        }
        
        private void OnCollisionStay2D(Collision2D other) {
            if (other.gameObject.CompareTag("Ground")) {
                ReduceMovement();
            }
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        public void SetRigidBodyVelocity(Vector2 velocity) {
            _rigidbody2D.velocity = velocity;
        }

        public void SetRigidBodyAngularVelocity(float velocity) {
            _rigidbody2D.angularVelocity = velocity;
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods

        private void ReduceMovement() {
            if (_rigidbody2D == null) return;
            _rigidbody2D.velocity *= 0.5f; // Reduce velocity
            _rigidbody2D.angularVelocity *= 0.5f; // Reduce angular velocity
            
            // Completely a velocity if it's below a threshold
            if (_rigidbody2D.velocity.magnitude < 0.1f) {
                _rigidbody2D.velocity = Vector2.zero;
            }
            if (Mathf.Abs(_rigidbody2D.angularVelocity) < 0.1f) {
                _rigidbody2D.angularVelocity = 0;
            }
        }
        
        #endregion
    }
}