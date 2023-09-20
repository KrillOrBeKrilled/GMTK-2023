using System;
using UnityEngine;

//*******************************************************************************************
// HeroData
//*******************************************************************************************
namespace KrillOrBeKrilled.Model {
    /// <summary>
    /// Stores data associated with a hero to spawn in a wave for a level. Contains
    /// data on the hero health, type, as well as their default values.
    /// </summary>
    [Serializable]
    public struct HeroData {
        [Tooltip("How much damage this hero can sustain before dying.")] 
        public int Health;
        [Tooltip("The type of hero that should be spawned; dictates the hero behaviour, strengths, and weaknesses " +
                 "during gameplay.")]
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
