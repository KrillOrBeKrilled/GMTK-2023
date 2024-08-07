using System;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************
// WaveData
//*******************************************************************************************
namespace KrillOrBeKrilled.Model {
    /// <summary>
    /// Stores data associated with a wave for a level to spawn a set of heroes.
    /// Contains <see cref="HeroData"/> for every hero in the wave, the duration of
    /// time to wait between spawning each hero, and the duration of time to wait
    /// before beginning the next wave.
    /// </summary>
    [Serializable]
    public struct WaveData {
        [Tooltip("List of heroes to spawn in this wave.")]
        public List<HeroData> Heroes;
        
        [Tooltip("Delay between spawning each hero.")]
        public float HeroSpawnDelayInSeconds;
    }
}
