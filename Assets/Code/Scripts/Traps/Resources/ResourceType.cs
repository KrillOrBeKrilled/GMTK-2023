using System;
using UnityEngine;

namespace KrillOrBeKrilled.Traps {
    public enum ResourceType {
        ScrapMetal,
        WoodStick,
        IceShards,
        Slime,
        Dynamite
    }
    
    //*******************************************************************************************
    // ResourceTypeIcon
    //*******************************************************************************************
    /// <summary>
    /// Stores data that specifies a resource type to an icon representation.
    /// </summary>
    [Serializable]
    public class ResourceTypeIcon {
        public ResourceType Type;
        public Sprite Icon;
    }

    //*******************************************************************************************
    // RecipeEntry
    //*******************************************************************************************
    /// <summary>
    /// For directly setting trap recipes in Unity Editor 
    /// </summary>
    [Serializable]
    public class ResourceEntry {
        public ResourceType type;
        public int amount;
    }
}
