using System;

namespace Model {
  [Serializable]
  public struct HeroData {
    public int Health;
    public HeroType Type;

    public enum HeroType {
      Default,
      AcidResistant,
      Armoured
    }
  }
}
