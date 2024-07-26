using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Traps;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// ResourceManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Manages the player's resource inventory. This includes displaying the Amount of
    /// resources the player has, and tracking the changes as the player collects them from
    /// pickups or spends them in building traps.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager> {
        private Dictionary<ResourceType, int> _inventory;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private IEnumerator Start() {
            // Skip a frame to wait for event listeners
            yield return null;
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(_inventory);
            
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
        /// Sets up subscriptions to player events required for proper execution.
        /// </summary>
        /// <param name="initialInventory"> The resources present in this level and the initial Amount given to the player. </param>
        /// <param name="onConsumeResources"> An event encapsulating when resources are requested to be consumed. </param>
        public void Initialize(List<ResourceAmount> initialInventory, UnityEvent<Dictionary<ResourceType, int>> onConsumeResources) {
            this._inventory = new Dictionary<ResourceType, int>();

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) {
                this._inventory[type] = 0;
            }

            foreach (ResourceAmount amount in initialInventory) {
                this._inventory[amount.Type] = amount.Amount;
            }
            onConsumeResources.AddListener(this.ConsumeResources);
        }
        
        /// <summary>
        /// Increase the Amount of a specific resource by a given quantity.
        /// </summary>
        /// <param name="type"> The Type of resource to add. </param>
        /// <param name="quantity"> The Amount of the resource to add. </param>
        public void AddResource(ResourceType type, int quantity) {
            if (!_inventory.ContainsKey(type)) {
                Debug.LogWarning("Invalid Resource Type for the current inventory.");
                return;
            }
            _inventory[type] += quantity;
            
            var updatedResource = new Dictionary<ResourceType, int> {
                { type, _inventory[type] }
            };
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(updatedResource);
        }

        /// <summary>
        /// Spends each resource by a certain Amount, as specified by the costs.
        /// Return false if any of the resources in the costs is not affordable.
        /// </summary>
        /// <param name="costs"> A dictionary of the resources and their costs. </param>
        public void ConsumeResources(Dictionary<ResourceType, int> costs) {
            if (!CanAffordCost(costs)) return;

            var updatedResources = new Dictionary<ResourceType, int>();
            foreach (var cost in costs) {
                _inventory[cost.Key] -= cost.Value;
                updatedResources[cost.Key] = _inventory[cost.Key];
            }
            
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(updatedResources);
        }
        
        /// <summary>
        /// Return true if the current inventory has enough resources to match the costs.
        /// </summary>
        /// <param name="costs"> The costs being checked. </param>
        public bool CanAffordCost(Dictionary<ResourceType, int> costs) {
            return costs.All(pair => _inventory[pair.Key] >= pair.Value);
        }
        
        #endregion
    }
}