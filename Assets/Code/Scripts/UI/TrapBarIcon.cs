using Traps;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class TrapBarIcon : MonoBehaviour {
    [SerializeField] private Image _selectionOutline;
    [SerializeField] private Image _tint;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    private Trap _assignedTrap;

    public void Initialize(Trap trap) {
      this._assignedTrap = trap;
    }

    public void OnSelectedChanged(bool isSelected) {
      this._selectionOutline.color = isSelected ? this._selectedColor : this._defaultColor;
    }

    public void OnCanAffordChanged(int newAmount) {
      this._tint.gameObject.SetActive(newAmount < this._assignedTrap.Cost);
    }
  }
}
