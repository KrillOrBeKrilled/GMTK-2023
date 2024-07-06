using System.Collections.Generic;
using KrillOrBeKrilled.Model;
using UnityEngine;
using UnityEngine.Serialization;

//*******************************************************************************************
// LevelManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Manages the movement of level data between scenes to differentiate level
    /// wave designs.
    /// </summary>
    public class LevelManager : Singleton<LevelManager> {
        [SerializeField] private LevelsList _levelsList;
        [FormerlySerializedAs("ActiveLevelData")] [SerializeField] private LevelData _activeLevelData;

        [FormerlySerializedAs("DefaultLevelData")]
        [Tooltip("Only used when starting Game scene directly, shouldn't be needed in build")]
        [SerializeField] private LevelData _defaultLevelData;

        private readonly Dictionary<string, LevelData> _levelDatas = new();
        private static bool LevelWasLoaded { get; set; }

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        protected override void Awake() {
            base.Awake();

            foreach (LevelData levelData in this._levelsList.LevelDatas) {
                this._levelDatas[levelData.name] = levelData;
            }
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Finds the active <see cref="LevelData"/> to be parsed in the current level.
        /// </summary>
        /// <returns> The active <see cref="LevelData"/> associated with the current level. </returns>
        /// <remarks>
        /// If the <see cref="LevelWasLoaded"/> is set between level loads, finds the copied <see cref="LevelData"/>
        /// structure associated with the level to be loaded. Otherwise, finds an assigned default
        /// <see cref="LevelData"/> to be loaded. (Only occurs in Editor play mode for the scene.)
        /// </remarks>
        public LevelData GetActiveLevelData() {
            return LevelManager.LevelWasLoaded ? this._activeLevelData : this._defaultLevelData;
        }

        /// <summary>
        /// Copies the <see cref="LevelData"/> associated with the level into a separate <see cref="LevelData"/>
        /// object to persist between scene loads for future parsing, and loads the game scene through the
        /// <see cref="SceneNavigationManager"/>.
        /// </summary>
        /// <param name="levelName"> The name of the level to load, used to find the associated
        /// <see cref="LevelData"/>. </param>
        public void LoadLevel(string levelName) {
            if (!this._levelDatas.ContainsKey(levelName)) {
                Debug.LogError($"Provided level name {levelName} is not present in level data source");
                return;
            }

            LevelData source = this._levelDatas[levelName];

            // Assign copy the values to avoid modifying data source and store them between scenes.
            // Note: stored data is not preserved between game sessions.
            LevelData.CopyData(source, ref this._activeLevelData);
            LevelManager.LevelWasLoaded = true;
            SceneNavigationManager.LoadGameLevelScene();
        }

        #endregion
    }
}
