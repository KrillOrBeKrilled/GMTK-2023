using System;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
    public enum ResourceType {
        ScrapMetal,
        WoodStick,
        IceShards,
        Slime,
        Dynamite
    }
    
    /// <summary>
    /// Stores data that specifies a resource Type to an icon representation.
    /// </summary>
    [Serializable]
    public class ResourceTypeIcon {
        public ResourceType Type;
        public Sprite Icon;
    }
}
