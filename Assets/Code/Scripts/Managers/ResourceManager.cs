using System;
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
            _inventory = new Dictionary<ResourceType, int>();
            
            // Initialize each resource type with a count of 0
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) {
                _inventory.Add(type, 5);
                EventManager.Instance.ResourceAmountChangedEvent.Invoke(type, _inventory[type]);
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
            
            string inventoryStr = "{";
            foreach (var pair in _inventory) {
                inventoryStr += $"{pair.Key}: {pair.Value}, ";
            }
            inventoryStr = inventoryStr.TrimEnd(' ', ',') + "}";

            print("Updated Inventory: " + inventoryStr);
            
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