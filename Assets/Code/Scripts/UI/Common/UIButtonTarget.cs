using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  [RequireComponent(typeof(Image))]
  public class UIButtonTarget : MonoBehaviour {
    [Tooltip("The target image that will play bounce animation and be changed on interaction.")]
    [SerializeField] private Image _image;
    [Tooltip("Should the button bounce on pointer down?")]
    [SerializeField] private bool _enableBounceAnimation = true;
    [Tooltip("The image shown to indicate the button is not pressed.")]
    [SerializeField] private Sprite _defaultImage;
    [Tooltip("The image shown to indicate the button is pressed.")]
    [SerializeField] private Sprite _pressedImage;
    [Tooltip("The image shown to indicate the button is disabled.")]
    [SerializeField] private Sprite _disabledImage;
    
    [HideInInspector] [SerializeField] private bool _isInteractable = true;
    private const float ScaleUpValue = 1.05f;
    private const float ScaleDownValue = 0.95f;
    private const float AnimationDuration = 0.05f;
    
    private Sequence _tweenSequence;
    private bool _isPressed;
    
    //========================================
    // Unity Methods
    //========================================
    
    /// <summary>
    /// Unity callback triggered when changes are made to the component in the editor.
    /// </summary>
    private void OnValidate() {
      if (this._image == null && this.TryGetComponent(out Image image)) {
        this._image = image;
      }
    }
    
    //========================================
    // Public Methods
    //========================================
    
    #region Public Methods

    /// <summary>
    /// Sets the isPressed property. 
    /// </summary>
    /// <param name="isPressed"> Whether the button pressed. </param>
    public void SetIsPressed(bool isPressed) {
      this._isPressed = isPressed;
    }
    
    /// <summary>
    /// Sets the set of the sprites that will be applied to the target image. 
    /// </summary>
    /// <param name="defaultImage"> The default image - shown by default. </param>
    /// <param name="pressedImage"> The pressed image - shown when button is interacted with. </param>
    /// <param name="disabledImage"> The disabled image - showm when button is no interactable. </param>
    public void SetButtonSprites(Sprite defaultImage, Sprite pressedImage, Sprite disabledImage = null) {
      this._defaultImage = defaultImage;
      this._pressedImage = pressedImage;
      this._disabledImage = disabledImage;

      if (!this._isInteractable) {
        this._image.sprite = disabledImage;
        return;
      }
            
      this._image.sprite = this._isPressed ? this._pressedImage : this._defaultImage;
    }

    /// <summary>
    /// Applies the Default sprite to the target image.
    /// </summary>
    public void ApplySpriteDefault() {
      this._image.sprite = this._defaultImage;
    }

    /// <summary>
    /// Applies the Pressed sprite to the target image.
    /// </summary>
    public void ApplySpritePressed() {
      this._image.sprite = this._pressedImage;
    }
    
    /// <summary>
    /// Sets this button to be interactable or un-interactable.
    /// If un-interactable - will try to set a disabled sprite.
    /// </summary>
    /// <param name="isInteractable"> Value to set the buttons interactable property to. </param>
    public void SetInteractable(bool isInteractable) {
      this._isInteractable = isInteractable;

      if (this._disabledImage != null) {
        this._image.sprite = this._isInteractable ? this._defaultImage : this._disabledImage;
      }
    }

    /// <summary>
    /// Scales the button up and back down to simulate a bounce animation.
    /// </summary>
    public void DoBounce() {
      if (!this._enableBounceAnimation) {
        return;
      }

      RectTransform target = this._image.rectTransform;
            
      this._tweenSequence?.Kill();
      this._tweenSequence = DOTween.Sequence();
      this._tweenSequence.Append(target.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
      this._tweenSequence.Append(target.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
      this._tweenSequence.Append(target.DOScale(Vector3.one, AnimationDuration));
      this._tweenSequence.OnComplete(() => this._tweenSequence = null);
      this._tweenSequence.SetEase(Ease.InOutSine);
      this._tweenSequence.Play();
    }
    
    #endregion
  }
}