using UnityEngine;
using UnityEngine.UI;

public class TrapBarIcon : MonoBehaviour {
  [SerializeField] private Image _selectionOutline;
  [SerializeField] private Image _tint;
  [SerializeField] private Color _selectedColor;
  [SerializeField] private Color _defaultColor;

  public void OnSelectedChanged(bool isSelected) {
    this._selectionOutline.color = isSelected ? this._selectedColor : this._defaultColor;
  }
}
