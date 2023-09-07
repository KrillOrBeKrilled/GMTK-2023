using Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers {
  public class LevelManager : Singleton<LevelManager> {
    [SerializeField] private LevelsList _levelsList;
    public LevelData ActiveLevelData;

    private readonly Dictionary<string, LevelData> _levelDatas = new Dictionary<string, LevelData>();

    public void LoadLevel(string levelName) {
      if (!this._levelDatas.ContainsKey(levelName)) {
        Debug.LogError($"Provided level name {levelName} is not present in level data source");
        return;
      }

      LevelData source = this._levelDatas[levelName];

      // Assign copy the values to avoid modifying data source and store them between scenes.
      // Note: stored data is not preserved between game sessions.
      this.ActiveLevelData.Type = source.Type;
      this.ActiveLevelData.DialogueName = source.DialogueName;
      this.ActiveLevelData.EndgameTargetPosition = source.EndgameTargetPosition;
      this.ActiveLevelData.RespawnPositions = source.RespawnPositions.ToList();
      this.ActiveLevelData.WavesData = new WavesData() { WavesList = source.WavesData.WavesList.ToList() };

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
