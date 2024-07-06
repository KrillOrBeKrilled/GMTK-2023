using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Sprite> ComicPages;
        public Vector3 EndgameTargetPosition = Vector3.zero;
        public Vector3 StartCameraPosition;
        public Vector3 EndCameraPosition;
        public List<Vector3> RespawnPositions;
        public Tilemap WallsTilemapPrefab;
        public WavesData WavesData;

        public enum LevelType {
            Story,
            Endless
        }

        public static void CopyData(LevelData source, ref LevelData destination) {
            destination.Index = source.Index;
            destination.DialogueName = source.DialogueName;
            destination.NextLevelName = source.NextLevelName;
            destination.ComicPages = source.ComicPages.ToList();
            destination.Type = source.Type;
            destination.EndgameTargetPosition = source.EndgameTargetPosition;
            destination.StartCameraPosition = source.StartCameraPosition;
            destination.EndCameraPosition = source.EndCameraPosition;
            destination.RespawnPositions = source.RespawnPositions.ToList();
            destination.WallsTilemapPrefab = source.WallsTilemapPrefab;
            destination.WavesData = new WavesData() { WavesList = source.WavesData.WavesList.ToList() };
        }
    }
}
