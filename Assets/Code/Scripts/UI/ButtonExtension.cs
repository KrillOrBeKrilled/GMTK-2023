using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//*******************************************************************************************
// ButtonExtension
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Creates additional events for a button to add extra visual effects and SFX.
    /// </summary>

    [RequireComponent(typeof(Image))]
    public class ButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
        [Tooltip("Tracks when the button is clicked.")]
        [SerializeField] private UnityEvent _onClick;
        [Tooltip("Tracks when the player cursor hovers over the button.")]
        [SerializeField] private UnityEvent _onHover;

        private const float ScaleUpValue = 1.1f;
        private const float ScaleDownValue = 0.9f;
        private const float AnimationDuration = 0.05f;
        private RectTransform _rectTransform;

        private Sequence _tweenSequence;

        /// <summary> Invokes the <see cref="_onHover"/> event. </summary>
        /// <param name="eventData"> The data associated with the button touch event. </param>
        public void OnPointerEnter(PointerEventData eventData) {
            this._onHover?.Invoke();
        }

        // The OnClicked event of the UI.Button component can also be used directly
        /// <summary> Invokes the <see cref="_onClick"/> event. </summary>
        /// <param name="eventData"> The data associated with the button touch event. </param>
        public void OnPointerClick(PointerEventData eventData) {
            this._onClick?.Invoke();
            this.PlayScaleBounceAnimation();
        }

        private void Awake() {
            this._rectTransform = this.GetComponent<RectTransform>();
        }

        private void PlayScaleBounceAnimation() {
            this._tweenSequence?.Kill();

            this._tweenSequence = DOTween.Sequence();
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
            this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
            this._tweenSequence.OnComplete(() => this._tweenSequence = null);
            this._tweenSequence.Play();
        }
    }
}
