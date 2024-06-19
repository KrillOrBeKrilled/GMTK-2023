using KrillOrBeKrilled.Heroes;
using UnityEngine;

namespace KrillOrBeKrilled.Common {
  [CreateAssetMenu(menuName = "Events/HeroEvent", fileName = "HeroEvent")]
  public class HeroEvent : GameEvent<Hero> {}
}