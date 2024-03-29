using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KrillOrBeKrilled.Core.Managers.Audio;

//*******************************************************************************************
// UIButton
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Customized UI Button, plays animation and SFX on tap.
    /// </summary>
    /// <remarks> Requires <see cref="Image"/> component. </remarks>

    [RequireComponent(typeof(Image))]
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler {
        [Tooltip("Dictates whether or not the button will play SFX when clicked.")]
        [SerializeField] private bool _muteClickSfx;
        [Tooltip("Should the button bounce on pointer down?")]
        [SerializeField] private bool _enableBounceAnimation = true;

        [Tooltip("Triggers when the button is pressed down and lifted.")]
        [SerializeField] private UnityEvent _onClick;
        
        [Tooltip("Target image that will play a bounce animation and change sprite on click")]
        [SerializeField] private Image _targetImage;
        [Tooltip("The image shown to indicate the button is not pressed.")]
        [SerializeField] private Sprite _defaultImage;
        [Tooltip("The image shown to indicate the button is pressed.")]
        [SerializeField] private Sprite _pressedImage;

        private const float ScaleUpValue = 1.05f;
        private const float ScaleDownValue = 0.95f;
        private const float AnimationDuration = 0.05f;

        private Sequence _tweenSequence;
        private bool _isInteractable = true;
        private bool _isPressed;
        private bool _isPointerOverButton;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        /// <summary>
        /// Triggered by Unity. Invokes <see cref="_onClick"/> event.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = false;
            this._targetImage.sprite = this._defaultImage;

            if (this._isPointerOverButton) {
                this._onClick?.Invoke();
            }

            this._isPointerOverButton = false;
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = true;
            this._isPointerOverButton = true;
            this._targetImage.sprite = this._pressedImage;

            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }
            
            this.DoBounce();
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = true;
            this._targetImage.sprite = this._pressedImage;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = false;
            this._targetImage.sprite = this._defaultImage;
        }

        private void OnValidate() {
            if (this._targetImage != null) {
                return;
            }
            
            if (this.TryGetComponent(out Image image)) {
                this._targetImage = image;
            }
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Changes the button sprites for the default and pressed images.
        /// </summary>
        public void SetButtonSprites(Sprite defaultImage, Sprite pressedImage) {
            this._defaultImage = defaultImage;
            this._pressedImage = pressedImage;

            this._targetImage.sprite = this._isPressed ? this._pressedImage : this._defaultImage;
        }

        /// <summary>
        /// Sets this button to be interactable or un-interactable.
        /// </summary>
        /// <param name="isInteractable"> Value to set the buttons interactable property to. </param>
        public void SetInteractable(bool isInteractable) {
            this._isInteractable = isInteractable;
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Scales the button up and back down to simulate a bounce animation.
        /// </summary>
        private void DoBounce() {
            if (!this._enableBounceAnimation) {
                return;
            }

            RectTransform target = this._targetImage.rectTransform;
            
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
