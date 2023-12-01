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

  [Serializable]
  public class RecipeEntry {
    public ResourceType type;
    public int amount;
  }
}
