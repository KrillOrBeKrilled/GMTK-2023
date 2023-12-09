using UnityEngine;

//*******************************************************************************************
// ResourcePickupData
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A ScriptableObject class to hold the fixed data of each resource pickup game object.
    /// This reduces the memory required to create separate data for the pickup objects.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourcePickupData", menuName = "UI Data/ResourcePickupData")]
    public class ResourcePickupData : ScriptableObject {
        public ResourceType resourceType;
        public int quantity;
    }
}