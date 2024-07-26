using System;

namespace KrillOrBeKrilled.Model {
  /// <summary>
  /// For directly setting trap recipes in Unity Editor 
  /// </summary>
  [Serializable]
  public class ResourceAmount {
    public ResourceType Type;
    public int Amount;
  }
}