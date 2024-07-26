using System;
using System.Collections.Generic;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Traps;
using UnityEngine;

//*******************************************************************************************
// ResourceUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Displays the player's inventory, containing the different types of resources and
    /// the Amount that the player currently has. 
    /// </summary>
    public class ResourceUI : MonoBehaviour {
        [SerializeField] private List<ResourceAmountUI> resourceIcons;
        private Dictionary<ResourceType, ResourceAmountUI> _inventory;
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Start() {
            _inventory = new Dictionary<ResourceType, ResourceAmountUI>();
            foreach (var icon in resourceIcons) {
                _inventory.TryAdd(icon.ResourceType, icon);
            }
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        public void SetAmount(ResourceType type, int amount) {
            _inventory[type].SetIconAmount(type, amount);
        }
    }
}