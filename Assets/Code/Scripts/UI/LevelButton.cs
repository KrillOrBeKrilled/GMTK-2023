using TMPro;
using UnityEngine;

//*******************************************************************************************
// LevelButton
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
  public class LevelButton : MonoBehaviour {
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private Sprite _completedSprite;
    [SerializeField] private Sprite _completedSpritePressed;
    [SerializeField] private Sprite _completedSpriteDisabled;
    
    [SerializeField] private Sprite _incompleteSprite;
    [SerializeField] private Sprite _incompleteSpritePressed;
    [SerializeField] private Sprite _incompleteSpriteDisabled;

    private UIButton _uiButton;

    //========================================
    // Unity Methods
    //========================================

    #region Unity Methods
    
    private void Awake() {
      this._uiButton = this.GetComponent<UIButton>();
    }
    
    #endregion

    //========================================
    // Public Methods
    //========================================
    
    #region Public Methods
    
    public void SetNumber(int index) {
      this._numberText.gameObject.SetActive(true);
      this._numberText.text = $"{index}";
    }

    public void EnableButton(bool isCompleted) {
      this._uiButton.SetInteractable(true);
      if (isCompleted) {
        this._uiButton.SetButtonSprites(this._completedSprite, this._completedSpritePressed, this._completedSpriteDisabled);
      } else {
        this._uiButton.SetButtonSprites(this._incompleteSprite, this._incompleteSpritePressed, this._incompleteSpriteDisabled);
      }
    }
    
    #endregion
  }
}