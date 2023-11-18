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
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField] private bool _muteClickSfx;

        [Tooltip("Invoked as soon as the button is pressed.")]
        [SerializeField] private UnityEvent _onClickImmediate;
        [Tooltip("Invoked after the press animation is completed.")]
        [SerializeField] private UnityEvent _onClickComplete;
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

        /// <summary> Triggered by Unity. Invokes <see cref="_onClickImmediate"/> event. </summary>
        public void OnPointerDown(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = true;
            this._onClickImmediate?.Invoke();
            this._image.sprite = this._pressedImage;

            this._tweenSequence?.Kill();
            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
            this._tweenSequence.OnComplete(() => {
                this._tweenSequence = null;

            });
            this._tweenSequence.SetEase(Ease.InOutSine);
            this._tweenSequence.Play();
        }

        /// <summary> Triggered by Unity. Invokes <see cref="_onClickComplete"/> event. </summary>
        public void OnPointerUp(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = false;
            this._image.sprite = this._defaultImage;
            this._onClickComplete?.Invoke();

            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }
        }

        private void Awake() {
            this._image = this.GetComponent<Image>();
            this._rectTransform = this.GetComponent<RectTransform>();
        }
    }
}
