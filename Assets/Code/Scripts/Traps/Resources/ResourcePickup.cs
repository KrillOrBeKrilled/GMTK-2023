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

        // Currently, using a static event to keep Managers depending on Traps.
        // Can be refactored later.
        public static event Action<ResourceType, int> OnResourceCollected;
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            OnResourceCollected?.Invoke(data.resourceType, data.quantity);
            Destroy(gameObject);
        }
    }
}