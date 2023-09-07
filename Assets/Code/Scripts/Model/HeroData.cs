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

    public static HeroData DefaultHero => new HeroData() {
      Health = 50,
      Type = HeroType.Default
    };
  }
}
