using System.Collections.Generic;
using KrillOrBeKrilled.Core.Managers;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class LevelsMapUI : MonoBehaviour {
    [SerializeField] private LevelButton _endlessLevel;
    [SerializeField] private List<LevelButton> _levels;
    
    private void Start() {
      this._endlessLevel.EnableButton(false);
      
      // Keep in mind, level indexes are stored starting from 1 in the data.
      foreach (int completedLevelIndex in DataManager.Instance.PlayerData.CompletedLevels) {
        this._levels[completedLevelIndex - 1].SetNumber(completedLevelIndex);
        this._levels[completedLevelIndex - 1].EnableButton(true);
      }

      int nextLevelIndex = DataManager.Instance.PlayerData.CompletedLevels.Count;
      if (nextLevelIndex <= this._levels.Count) {
        this._levels[nextLevelIndex].SetNumber(nextLevelIndex + 1);
        this._levels[nextLevelIndex].EnableButton(false);
      }
    }
  }
}