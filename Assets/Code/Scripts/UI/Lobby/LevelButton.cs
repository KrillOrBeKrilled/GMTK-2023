using TMPro;
using UnityEngine;

//*******************************************************************************************
// LevelButton
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
  /// <summary>
  /// Custom button, updates the completed/incomplete sprites on the sibling UIButton component.
  /// </summary>
  public class LevelButton : MonoBehaviour {
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private Sprite _completedSprite;
    [SerializeField] private Sprite _completedSpritePressed;
    [SerializeField] private Sprite _completedSpriteDisabled;
    
    [SerializeField] private Sprite _incompleteSprite;
    [SerializeField] private Sprite _incompleteSpritePressed;
    [SerializeField] private Sprite _incompleteSpriteDisabled;

    private UIButton _uiButton;
    private UIButtonTarget _uiButtonTarget;

    //========================================
    // Unity Methods
    //========================================

    #region Unity Methods
    
    private void Awake() {
      this._uiButton = this.GetComponent<UIButton>();
      this._uiButtonTarget = this.GetComponent<UIButtonTarget>();
    }
    
    #endregion

    //========================================
    // Public Methods
    //========================================
    
    #region Public Methods
    
    /// <summary>
    /// Sets the number of the level this button corresponds to.
    /// </summary>
    /// <param name="index"> the number of the level. </param>
    public void SetNumber(int index) {
      this._numberText.gameObject.SetActive(true);
      this._numberText.text = $"{index}";
    }

    /// <summary>
    /// LevelButton is disabled by default. This method enables it and sets correct sprite based on
    /// the argument provided.
    /// </summary>
    /// <param name="isCompleted"> Whether the player has already completed this level. </param>
    public void EnableButton(bool isCompleted) {
      this._uiButton.SetInteractable(true);
      if (isCompleted) {
        this._uiButtonTarget.SetButtonSprites(this._completedSprite, this._completedSpritePressed, this._completedSpriteDisabled);
      } else {
        this._uiButtonTarget.SetButtonSprites(this._incompleteSprite, this._incompleteSpritePressed, this._incompleteSpriteDisabled);
      }
    }
    
    #endregion
  }
}