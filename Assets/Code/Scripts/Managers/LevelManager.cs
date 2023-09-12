using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace KrillOrBeKrilled.Managers {
    public class LevelManager : Singleton<LevelManager> {
        [SerializeField] private LevelsList _levelsList;
        [FormerlySerializedAs("ActiveLevelData")] [SerializeField] private LevelData _activeLevelData;

        [FormerlySerializedAs("DefaultLevelData")]
        [Tooltip("Only used when starting Game scene directly, shouldn't be needed in build")]
        [SerializeField] private LevelData _defaultLevelData;

        private readonly Dictionary<string, LevelData> _levelDatas = new Dictionary<string, LevelData>();
        private static bool LevelWasLoaded { get; set; } = false;

        public LevelData GetActiveLevelData() {
            return LevelWasLoaded ? this._activeLevelData : this._defaultLevelData;
        }

        public void LoadLevel(string levelName) {
            if (!this._levelDatas.ContainsKey(levelName)) {
                Debug.LogError($"Provided level name {levelName} is not present in level data source");
                return;
            }

            LevelData source = this._levelDatas[levelName];

            // Assign copy the values to avoid modifying data source and store them between scenes.
            // Note: stored data is not preserved between game sessions.
            this._activeLevelData.Type = source.Type;
            this._activeLevelData.DialogueName = source.DialogueName;
            this._activeLevelData.EndgameTargetPosition = source.EndgameTargetPosition;
            this._activeLevelData.RespawnPositions = source.RespawnPositions.ToList();
            this._activeLevelData.WavesData = new WavesData() { WavesList = source.WavesData.WavesList.ToList() };

            LevelWasLoaded = true;
            SceneNavigationManager.Instance.LoadGameScene();
        }

        protected override void Awake() {
            base.Awake();

            foreach (LevelData levelData in this._levelsList.LevelDatas) {
                this._levelDatas[levelData.name] = levelData;
            }
        }
    }
}
