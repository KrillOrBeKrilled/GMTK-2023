using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Common {
  [CreateAssetMenu(menuName = "Events/SpriteListEvent", fileName = "SpriteListEvent")]
  public class SpriteListEvent : GameEvent<List<Sprite>> {}
}