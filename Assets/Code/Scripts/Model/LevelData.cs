using System;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
    [Serializable]
    public class LevelData : ScriptableObject {
        public LevelType Type = LevelType.Endless;
        public string DialogueName;
        public Vector3 EndgameTargetPosition = Vector3.zero;
        public List<Vector3> RespawnPositions;
        public WavesData WavesData;

        public enum LevelType {
            Story,
            Endless
        }
    }
}
