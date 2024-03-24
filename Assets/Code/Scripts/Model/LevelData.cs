using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// LevelData
//*******************************************************************************************
namespace KrillOrBeKrilled.Model {
    /// <summary>
    /// Stores data associated with a level to generate waves of heroes. Contains
    /// data on <see cref="LevelType"/>, the associated story dialogue to reference,
    /// the hero goal to reach for game over, hero spawn locations, and
    /// <see cref="WavesData"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
    [Serializable]
    public class LevelData : ScriptableObject {
        public int Index;
        [Tooltip("The default level mode is Endless.")]
        public LevelType Type = LevelType.Endless;
        public string DialogueName;
        public string NextLevelName;
        public Vector3 EndgameTargetPosition = Vector3.zero;
        public List<Vector3> RespawnPositions;
        public Tilemap WallsTilemapPrefab;
        public WavesData WavesData;

        public enum LevelType {
            Story,
            Endless
        }
    }
}
