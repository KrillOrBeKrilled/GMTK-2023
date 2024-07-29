using System;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
  /// <summary>
  /// Stores a hero Type and a list of potential resource drops.
  /// </summary>
  [Serializable]
  public class HeroDrop {
    [Tooltip("The hero Type.")]
    public HeroType heroType;
    [Tooltip("The list of potential resource drops.")]
    public List<ResourceDrop> drops;
  }
}