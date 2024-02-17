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
    /// Customized UI HoldButton, plays animation and SFX on tap.
    /// Triggers an event if held for required time.
    /// </summary>
    /// <remarks> Requires <see cref="Image"/> component. </remarks>

    [RequireComponent(typeof(Image))]
    public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [Tooltip("Duration of time to hold the button to register its click completion event.")]
        [SerializeField] private float _holdTime = 1.5f;
        [Tooltip("Dictates whether or not the button will play SFX when clicked.")]
        [SerializeField] private bool _muteClickSfx;
        [Tooltip("Dictates whether or not the button will be disabled after its click completion event.")]
        [SerializeField] private bool _hideOnCompletion;
        [Tooltip("Triggers when the button is held for the hold time duration.")]
        [SerializeField] private UnityEvent _onHoldComplete;
        [Tooltip("The image shown to indicate the button is not pressed.")]
        [SerializeField] private Sprite _defaultImage;
        [Tooltip("The image shown to indicate the button is pressed.")]
        [SerializeField] private Sprite _pressedImage;

        private const float ScaleDownValue = 0.8f;
        private const float ScaleUpTime = 1f;
        private const float TriggeredScaleUpTime = 0.3f;
        private RectTransform _rectTransform;
        private Image _image;

        private Sequence _tweenSequence;
        private bool _isInteractable = true;
        private bool _eventTriggered;
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            this._image = this.GetComponent<Image>();
            this._rectTransform = this.GetComponent<RectTransform>();
        }
        
        public void OnPointerUp(PointerEventData eventData) {
            this._image.sprite = this._defaultImage;

            this._tweenSequence?.Kill();
            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, ScaleUpTime));
            this._tweenSequence.SetEase(Ease.OutBack);
            this._tweenSequence.OnComplete(() => {
                this._tweenSequence = null;
                if (this._eventTriggered && this._hideOnCompletion) {
                    this.gameObject.SetActive(false);
                }
            });
            this._tweenSequence.Play();
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._image.sprite = this._pressedImage;

            this._tweenSequence?.Kill();
            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(
                this._rectTransform
                    .DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), this._holdTime)
                    .SetEase(Ease.OutCubic)
            );
            this._tweenSequence.AppendCallback(() => {
                this._eventTriggered = true;
                this._onHoldComplete?.Invoke();
            });
            this._tweenSequence.Append(
                this._rectTransform
                    .DOScale(Vector3.one, TriggeredScaleUpTime)
                    .SetEase(Ease.OutBack)
            );
            this._tweenSequence.OnComplete(() => {
                this._tweenSequence = null;
                if (this._hideOnCompletion) {
                    this.gameObject.SetActive(false);
                }
            });
            this._tweenSequence.Play();
            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }
        }

        #endregion

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Hides this HoldButton once the OnPointerUp tween is complete.
        /// If no tween is running will hide immediately.
        /// </summary>
        public void HideOnCompletion() {
            this._hideOnCompletion = true;
            if (this._tweenSequence == null) {
                this.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Sets this button to be interactable or not-interactable.
        /// </summary>
        /// <param name="isInteractable"> Value to set the buttons interactable property to. </param>
        public void SetInteractable(bool isInteractable) {
            this._isInteractable = isInteractable;
        }
        
        #endregion
    }
}
