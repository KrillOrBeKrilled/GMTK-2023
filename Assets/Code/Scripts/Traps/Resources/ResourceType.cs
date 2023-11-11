using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KrillOrBeKrilled.Traps {
  public enum ResourceType {
    ScrapMetal,
    WoodStick,
    IceShards,
    Slime,
    Dynamite
  }

  [Serializable]
  public class ResourceTypeIcon {
    public ResourceType Type;
    public Sprite Icon;
  }

  [CreateAssetMenu(fileName = "ResourceTypeData", menuName = "UI Data/ResourceTypeData")]
  public class ResourceTypeData : ScriptableObject {
    [SerializeField] private List<ResourceTypeIcon> _icons;

    public Sprite TypeToImage(ResourceType type) {
      return this._icons.First(typeIcon => typeIcon.Type == type).Icon;
    }
  }
}
