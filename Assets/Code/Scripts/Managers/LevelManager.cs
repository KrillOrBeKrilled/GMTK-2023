using Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers {
  public class LevelManager : Singleton<LevelManager> {
    [SerializeField] private LevelsList _levelsList;

    private readonly Dictionary<string, LevelData> _levelDatas = new Dictionary<string, LevelData>();
    private LevelData _activeLevelData;

    public void LoadLevel(string levelName) {
      if (!this._levelDatas.ContainsKey(levelName)) {
        Debug.LogError($"Provided level name {levelName} is not present in level data source");
        return;
      }

      this._activeLevelData = this._levelDatas[levelName];
      SceneNavigationManager.Instance.LoadGameScene();
    }

    protected override void Awake() {
      base.Awake();

      foreach (LevelData levelData in this._levelsList.LevelDatas) {
        this._levelDatas[levelData.name] = levelData;
      }

      DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
      SceneNavigationManager.Instance.OnSceneLoaded.AddListener(this.OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
      if (scene.name != "Game") {
        return;
      }

      GameManager.Instance.Initialize(this._activeLevelData);
    }
  }
}
