using System;
using System.Collections.Generic;
using KrillOrBeKrilled.Traps;
using UnityEngine;

//*******************************************************************************************
// ResourceMagnet
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Detects resource pickups within a given range and sets velocity of the pickups
    /// for attraction towards the player.
    /// </summary>
    public class ResourceMagnet : MonoBehaviour {
        [Tooltip("The range within which the pickups can be attracted.")]
        [SerializeField] private float _attractionRadius;
        [Tooltip("Maximum speed at which pickups will move towards the player.")]
        [SerializeField] private float _maxSpeed;
        [Tooltip("Minimum speed at which pickups will move towards the player.")]
        [SerializeField] private float _minSpeed;
        [Tooltip("Maximum number of objects that the magnet can process.")]
        [SerializeField] private int _maxObjectsToProcess;

        private Collider2D[] hitColliders;
        private List<ResourcePickup> _pickups;
        
        //========================================
        // Unity Methods
        //========================================

        private void Awake() {
            hitColliders = new Collider2D[_maxObjectsToProcess];
            _pickups = new List<ResourcePickup>();
        }

        private void FixedUpdate() {
            AttractPickups();
        }

        //========================================
        // Private Methods
        //========================================

        private void AttractPickups() {
            int size = Physics2D.OverlapCircleNonAlloc(transform.position, _attractionRadius, hitColliders);
            for (int i = 0; i < size; i++) {
                if (hitColliders[i] == null) continue;

                ResourcePickup pickup = hitColliders[i].GetComponent<ResourcePickup>();
                if (pickup != null)
                {
                    Vector2 directionToPlayer = (Vector2)(transform.position - pickup.transform.position);
                    float distanceToPlayer = directionToPlayer.magnitude;
                    
                    // Choose a speed based on the pickup's distance to the player
                    float speed = Mathf.Lerp(_minSpeed, _maxSpeed, distanceToPlayer / _attractionRadius);
                    
                    // For safety, double check that speed is in bound
                    speed = Mathf.Clamp(speed, _minSpeed, _maxSpeed);
                    
                    pickup.SetRigidBodyVelocity(directionToPlayer.normalized * speed);
                }
            }
            
            Array.Clear(hitColliders, 0, hitColliders.Length);  // clear the array for safety
        }

        // For debugging
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _attractionRadius);
        }
    }
}