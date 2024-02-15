using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Traps;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// ResourceManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Manages the player's resource inventory. This includes displaying the amount of
    /// resources the player has, and tracking the changes as the player collects them from
    /// pickups or spends them in building traps.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager> {
        [Tooltip("The resources present in this level and the initial amount given to the player.")]
        [SerializeField] private List<ResourceEntry> initialInventory; 
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
        private IEnumerator DelayedInventoryInit() {
            _inventory = new Dictionary<ResourceType, int>();
            foreach (var entry in initialInventory) {
                _inventory.Add(entry.type, entry.amount);
            }
            
            // Skip a frame to wait for event listeners
            yield return null;
            EventManager.Instance.ResourceAmountChangedEvent.Invoke(_inventory);
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================

        #region Public Methods
        
        public void Initialize(UnityEvent<Dictionary<ResourceType, int>> onConsumeResources) {
            onConsumeResources.AddListener(this.ConsumeResources);
        }
        
        /// <summary>
        /// Increase the amount of a specific resource by a given quantity.
        /// </summary>
        /// <param name="type"> The type of resource to add. </param>
        /// <param name="quantity"> The amount of the resource to add. </param>
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
        /// Spends each resource by a certain amount, as specified by the costs.
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