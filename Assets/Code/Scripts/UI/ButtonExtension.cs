using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//*******************************************************************************************
// ButtonExtension
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Creates additional events for a button to add extra visual effects and SFX.
    /// </summary>
    public class ButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
        [Tooltip("Tracks when the button is clicked.")]
        [SerializeField] private UnityEvent _onClick;
        [Tooltip("Tracks when the player cursor hovers over the button.")]
        [SerializeField] private UnityEvent _onHover;

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
        }
    }
}
