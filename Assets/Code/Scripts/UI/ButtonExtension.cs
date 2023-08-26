using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI {
  public class ButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
    [SerializeField] private UnityEvent _onClick;
    [SerializeField] private UnityEvent _onHover;

    public void OnPointerEnter(PointerEventData eventData) {
      this._onHover?.Invoke();
    }

    // The OnClicked event of the UI.Button component can also be used directly
    public void OnPointerClick(PointerEventData eventData) {
      this._onClick?.Invoke();
    }
  }
}
