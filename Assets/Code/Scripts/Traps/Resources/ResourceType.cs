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

  [Serializable]
  public class ResourceTypeIcon {
    public ResourceType Type;
    public Sprite Icon;
  }
}
