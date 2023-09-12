using System;

namespace Model {
  [Serializable]
  public struct HeroData {
    public int Health;
    public HeroType Type;

    public enum HeroType {
      Default,
      Druid,
      AcidResistant,
      Armoured
    }

    public static HeroData DefaultHero => new HeroData() {
      Health = 10,
      Type = HeroType.Default
    };
  }
}
