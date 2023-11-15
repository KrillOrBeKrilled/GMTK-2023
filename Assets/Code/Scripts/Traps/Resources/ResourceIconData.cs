using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KrillOrBeKrilled.Traps {
  [CreateAssetMenu(fileName = "ResourceIconData", menuName = "UI Data/ResourceIconData")]
  public class ResourceIconData : ScriptableObject {
    [SerializeField] private List<ResourceTypeIcon> _icons;

    public Sprite TypeToImage(ResourceType type) {
      return this._icons.First(typeIcon => typeIcon.Type == type).Icon;
    }
  }
}
