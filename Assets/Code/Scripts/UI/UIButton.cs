using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KrillOrBeKrilled.Managers.Audio;

//*******************************************************************************************
// UIButton
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Customized UI Button, plays animation and SFX on tap.
    /// </summary>
    /// <remarks>Requires <see cref="Image"/> component.</remarks>

    [RequireComponent(typeof(Image))]
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler {
        [SerializeField] private bool _muteClickSfx;

        [SerializeField] private UnityEvent _onClick;
        [SerializeField] private Sprite _defaultImage;
        [SerializeField] private Sprite _pressedImage;

        private const float ScaleUpValue = 1.05f;
        private const float ScaleDownValue = 0.95f;
        private const float AnimationDuration = 0.05f;
        private RectTransform _rectTransform;
        private Image _image;

        private Sequence _tweenSequence;
        private bool _isInteractable = true;
        private bool _isPressed;
        private bool _isPointerOverButton;

        /// <summary>
        /// Sets this button to be interactable or un-interactable.
        /// </summary>
        /// <param name="isInteractable">Value to set the buttons interactable property to.</param>
        public void SetInteractable(bool isInteractable) {
            this._isInteractable = isInteractable;
        }

        /// <summary>
        /// Changes the button sprites for the default and pressed images.
        /// </summary>
        public void SetButtonSprites(Sprite defaultImage, Sprite pressedImage) {
            this._defaultImage = defaultImage;
            this._pressedImage = pressedImage;

            this._image.sprite = this._isPressed ? this._pressedImage : this._defaultImage;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = true;
            this._isPointerOverButton = true;
            this._image.sprite = this._pressedImage;

            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }

            this.DoBounce();
        }

        /// <summary> Triggered by Unity. Invokes <see cref="_onClick"/> event. </summary>
        public void OnPointerUp(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = false;
            this._image.sprite = this._defaultImage;

            if (this._isPointerOverButton) {
                this._onClick?.Invoke();
            }

            this._isPointerOverButton = false;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = false;
        }

        private void Awake() {
            this._image = this.GetComponent<Image>();
            this._rectTransform = this.GetComponent<RectTransform>();
        }

        private void DoBounce() {
            this._tweenSequence?.Kill();
            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
            this._tweenSequence.OnComplete(() => this._tweenSequence = null);
            this._tweenSequence.SetEase(Ease.InOutSine);
            this._tweenSequence.Play();
        }
    }
}
