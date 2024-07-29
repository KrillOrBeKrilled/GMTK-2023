using KrillOrBeKrilled.Model;
using UnityEngine;

//*******************************************************************************************
// ResourcePickupData
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A ScriptableObject class to hold the fixed data of each resource pickup game object.
    /// This reduces the memory required to create separate data for the pickup objects.
    /// </summary>
    public struct ResourcePickupData {
        public readonly ResourceType ResourceType;
        public readonly int Quantity;
        
        public ResourcePickupData(ResourceType resourceType, int quantity) {
            this.ResourceType = resourceType;
            this.Quantity = quantity;
        }
    }
}