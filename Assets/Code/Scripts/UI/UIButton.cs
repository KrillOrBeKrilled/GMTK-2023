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
    public class UIButton : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private bool _muteClickSfx;

        [Tooltip("Invoked as soon as the button is pressed.")]
        [SerializeField] private UnityEvent _onClickImmediate;
        [Tooltip("Invoked after the press animation is completed.")]
        [SerializeField] private UnityEvent _onClickComplete;

        private const float ScaleUpValue = 1.1f;
        private const float ScaleDownValue = 0.9f;
        private const float AnimationDuration = 0.07f;
        private RectTransform _rectTransform;

        private Sequence _tweenSequence;
        private bool _isInteractable = true;

        /// <summary>
        /// Sets this button to be interactable or un-interactable.
        /// </summary>
        /// <param name="isInteractable">Value to set the buttons interactable property to.</param>
        public void SetInteractable(bool isInteractable) {
            this._isInteractable = isInteractable;
        }

        // The OnClicked event of the UI.Button component can also be used directly
        /// <summary> Invokes the <see cref="_onClick"/> event. </summary>
        /// <param name="eventData"> The data associated with the button touch event. </param>
        public void OnPointerClick(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._onClickImmediate?.Invoke();
            this._tweenSequence?.Kill();

            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
            this._tweenSequence.OnComplete(() => {
                this._tweenSequence = null;
                this._onClickComplete?.Invoke();
            });
            this._tweenSequence.SetEase(Ease.InOutSine);
            this._tweenSequence.Play();

            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }
        }

        private void Awake() {
            this._rectTransform = this.GetComponent<RectTransform>();
        }
    }
}
