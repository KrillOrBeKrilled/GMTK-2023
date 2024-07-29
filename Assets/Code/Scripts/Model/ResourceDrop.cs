using System;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
  /// <summary>
  /// Stores a particular trap material's Type, its prefab, and a drop weight (higher weight
  /// means more likely to drop).
  /// </summary>
  [Serializable]
  public class ResourceDrop {
    [Tooltip("The Type of the dropped resource.")]
    public ResourceType resourceType;
    [Tooltip("The higher the weight, the more likely to drop.")]
    public int weight;
  }
}