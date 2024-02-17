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
    }
}