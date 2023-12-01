using System;
using System.Collections.Generic;
using KrillOrBeKrilled.Traps;

//*******************************************************************************************
// ResourceManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Manages the player's resource inventory. This includes displaying the amount of
    /// resources the player has, and tracking the changes as the player collects them from
    /// pickups or spends them in building traps.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager> {
        private Dictionary<ResourceType, int> _resources;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            _resources = new Dictionary<ResourceType, int>();
            
            // Initialize each resource type with a count of 0
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) {
                _resources.Add(type, 0);
            }
            
            // Subscribe to resource collection event in ResourcePickup
            ResourcePickup.OnResourceCollected += AddResource;
        }

        private void OnDestroy() {
            // Unsubscribe to avoid memory leaks
            ResourcePickup.OnResourceCollected -= AddResource;
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================

        #region Public Methods
        
        /// <summary>
        /// Increase the amount of a specific resource by a given quantity.
        /// </summary>
        /// <param name="type"> The type of resource to add. </param>
        /// <param name="quantity"> The amount of the resource to add. </param>
        public void AddResource(ResourceType type, int quantity) {
            _resources[type] += quantity;
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(type, _resources[type]);
        }

        /// <summary>
        /// Spend a specific resource by a given quantity.
        /// Return true if this succeeds. Return false if the amount of remaining resource
        /// is less than the quantity to spend.
        /// </summary>
        /// <param name="type"> The type of resource to spend. </param>
        /// <param name="quantity"> The amount of the resource to spend. </param>
        public bool ConsumeResource(ResourceType type, int quantity) {
            if (_resources[type] < quantity) {
                return false;
            }
            _resources[type] -= quantity;
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(type, _resources[type]);
            return true;
        }

        /// <summary>
        /// Return the current count of a specific resource type.
        /// </summary>
        /// <param name="type"> The type of resource being queried for the amount. </param>
        public int GetResourceQuantity(ResourceType type) {
            return _resources[type];
        }
        
        #endregion
    }
}