using System;
using System.Collections.Generic;

namespace Model {
  [Serializable]
  public struct WaveData {
    /// <summary>
    /// List of heroes to spawn in this wave.
    /// </summary>
    public List<HeroData> Heroes;

    /// <summary>
    /// Delay between spawning each hero.
    /// </summary>
    public float HeroSpawnDelayInSeconds;

    /// <summary>
    /// After finishing this wave, how many seconds should we wait before spawning next one.<br></br>
    /// Note: clearing all heroes of this wave will trigger spawning of next wave regardless of this delay.
    /// </summary>
    public float NextWaveSpawnDelayInSeconds;
  }
}
