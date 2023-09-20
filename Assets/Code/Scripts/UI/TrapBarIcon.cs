using KrillOrBeKrilled.Traps;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// TrapBarIcon
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages the animations associated with a single trap icon, rendering outlines
    /// upon trap selection and tinting out the icon when the corresponding trap is
    /// not affordable.
    /// </summary>
    public class TrapBarIcon : MonoBehaviour {
        [SerializeField] private Image _selectionOutline;
        [SerializeField] private Image _tint;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;

        private Trap _assignedTrap;

        /// <summary> Sets a reference to the trap this icon represents. </summary>
        /// <param name="trap"> The <see cref="Trap"/> associated with the trap type prefab. </param>
        public void Initialize(Trap trap) {
            this._assignedTrap = trap;
        }

        /// <summary> Outlines this icon if the corresponding trap is currently selected. </summary>
        /// <param name="isSelected"> If the trap type associated with this icon is currently selected. </param>
        public void OnSelectedChanged(bool isSelected) {
            this._selectionOutline.color = isSelected ? this._selectedColor : this._defaultColor;
        }

        /// <summary> Adds a tint to this icon if the corresponding trap is not affordable. </summary>
        /// <param name="newAmount"> The current number of coins available. </param>
        public void OnCanAffordChanged(int newAmount) {
            this._tint.gameObject.SetActive(newAmount < this._assignedTrap.Cost);
        }
    }
}
