using System.Collections.Generic;
using KrillOrBeKrilled.Core.Managers;
using UnityEngine;

//*******************************************************************************************
// LevelsMapUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
  /// <summary>
  /// Controls the display of the Levels map, level buttons, and map pages.
  /// </summary>
  public class LevelsMapUI : MonoBehaviour {
    [SerializeField] private LevelButton _endlessLevel;
    [SerializeField] private List<LevelButton> _levels;

    [SerializeField] private UIButton _leftArrow;
    [SerializeField] private UIButton _rightArrow;
    [SerializeField] private List<GameObject> _mapPagesBackgrounds;
    [SerializeField] private List<GameObject> _mapPages;
    private int _mapPageIndex;
    
    //========================================
    // Unity Methods
    //========================================

    #region Unity Methods
    
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
    
    #endregion
    
    //========================================
    // Public Methods
    //========================================
    
    #region Public Methods
    
    /// <summary>
    /// Displays the next map page on the UI.
    /// </summary>
    public void SelectNextPage() {
      this._mapPages[this._mapPageIndex].SetActive(false);
      this._mapPagesBackgrounds[this._mapPageIndex].SetActive(false);
      this._mapPageIndex = Mathf.Clamp(this._mapPageIndex + 1, 0, this._mapPages.Count);
      this._mapPages[this._mapPageIndex].SetActive(true);
      this._mapPagesBackgrounds[this._mapPageIndex].SetActive(true);

      this._endlessLevel.gameObject.SetActive(this._mapPageIndex == 0);
      
      this.UpdatePageArrows();
    }
    
    /// <summary>
    /// Displays the previous map page on the UI.
    /// </summary>
    public void SelectPreviousPage() {
      this._mapPages[this._mapPageIndex].SetActive(false);
      this._mapPagesBackgrounds[this._mapPageIndex].SetActive(false);
      this._mapPageIndex = Mathf.Clamp(this._mapPageIndex - 1, 0, this._mapPages.Count);
      this._mapPages[this._mapPageIndex].SetActive(true);
      this._mapPagesBackgrounds[this._mapPageIndex].SetActive(true);
      
      this._endlessLevel.gameObject.SetActive(this._mapPageIndex == 0);
      
      this.UpdatePageArrows();
    }
    
    #endregion


    //========================================
    // Private Methods
    //========================================
    
    #region Private Methods
    
    /// <summary>
    /// Enables/Disables arrow buttons depending on whether it is the first or the last page.
    /// </summary>
    private void UpdatePageArrows() {
      this._leftArrow.SetInteractable(this._mapPageIndex > 0);
      this._rightArrow.SetInteractable(this._mapPageIndex < this._mapPages.Count - 1);
    }
    
    #endregion
  }
}