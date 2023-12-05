using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<ResourceType, int> _inventory;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            StartCoroutine(DelayedInventoryInit());
            
            // Subscribe to resource collection event in ResourcePickup
            ResourcePickup.OnResourceCollected += AddResource;
        }

        private void OnDestroy() {
            // Unsubscribe to avoid memory leaks
            ResourcePickup.OnResourceCollected -= AddResource;
        }

        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// A coroutine to delay the initialization of the player inventory so that the
        /// subscribers can be set up first.
        /// </summary>
        /// <remarks>
        /// ResourceAmountChangedEvent is tied to a specific ResourceType, so we need to
        /// individually update each key-value pair. However, the GameUI will be checking
        /// for all keys when it updates the resource counts, so we will first initialize
        /// all keys with value 0 before assigning desired values to them.
        /// </remarks>
        private IEnumerator DelayedInventoryInit() {
            // Initialize keys first
            _inventory = new Dictionary<ResourceType, int>();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) {
                _inventory.Add(type, 0);
            }
            
            // Skip a frame to wait for event listeners
            yield return null;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) {
                _inventory[type] = 5;
                EventManager.Instance.ResourceAmountChangedEvent.Invoke(type, _inventory[type]);
            }
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
            _inventory[type] += quantity;
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(type, _inventory[type]);
        }

        /// <summary>
        /// Spends each resource by a certain amount, as specified by the costs.
        /// Return false if any of the resources in the costs is not affordable.
        /// </summary>
        /// <param name="costs"> A dictionary of the resources and their costs. </param>
        public bool ConsumeResources(Dictionary<ResourceType, int> costs) {
            if (!CanAffordCost(costs)) return false;

            foreach (var cost in costs) {
                _inventory[cost.Key] -= cost.Value;
                EventManager.Instance.ResourceAmountChangedEvent.Invoke(cost.Key, _inventory[cost.Key]);
            }
            return true;
        }
        
        /// <summary>
        /// Return true if the current inventory has enough resources to match the costs.
        /// </summary>
        /// <param name="costs"> The costs being checked. </param>
        public bool CanAffordCost(Dictionary<ResourceType, int> costs) {
            return costs.All(pair => _inventory[pair.Key] >= pair.Value);
        }
        
        /// <summary>
        /// Return the current count of a specific resource type.
        /// </summary>
        /// <param name="type"> The type of resource being queried for the amount. </param>
        public int GetResourceQuantity(ResourceType type) {
            return _inventory[type];
        }
        
        #endregion
        
        
    }
}