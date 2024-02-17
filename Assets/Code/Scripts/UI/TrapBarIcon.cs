using DG.Tweening;
using KrillOrBeKrilled.Core.Managers;
using KrillOrBeKrilled.Traps;
using UnityEngine;
using UnityEngine.Events;
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
        [Tooltip("Outlines the trap icon and changes color according to the selected and unselected states.")]
        [SerializeField] private Image _selectionOutline;
        [Tooltip("Covers the trap icon when the trap cannot be afforded.")]
        [SerializeField] private Image _tint;
        [Tooltip("The outline color when the associated trap is selected for deployment.")]
        [SerializeField] private Color _selectedColor;
        [Tooltip("The outline color when the associated trap is not selected for deployment.")]
        [SerializeField] private Color _defaultColor;

        private UnityAction<Trap> _selectTrapAction;
        private Trap _assignedTrap;
        private Tween _colorTween;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Sets a reference to the trap this icon represents.
        /// </summary>
        /// <param name="trap"> The <see cref="Trap"/> associated with the trap type prefab. </param>
        /// <param name="selectTrapAction"> The callback this UI Icon invokes upon being clicked. </param>
        public void Initialize(Trap trap, UnityAction<Trap> selectTrapAction) {
            this._assignedTrap = trap;
            this._selectTrapAction = selectTrapAction;
        }

        // /// <summary>
        // /// Adds a tint to this icon if the corresponding trap is not affordable.
        // /// </summary>
        // /// <param name="newAmount"> The current number of coins available. </param>
        // public void OnCanAffordChanged(int newAmount) {
        //     this._tint.gameObject.SetActive(newAmount < this._assignedTrap.Cost);
        // }
        
        /// <summary>
        /// Adds a tint to this icon if the corresponding trap is not affordable.
        /// </summary>
        public void CheckAffordable() {
            bool canAfford = ResourceManager.Instance.CanAffordCost(this._assignedTrap.Recipe);
            this._tint.gameObject.SetActive(!canAfford);
        }
        
        /// <summary>
        /// Outlines this icon if the corresponding trap is currently selected.
        /// </summary>
        /// <param name="newTrap"> The newly selected trap. </param>
        public void OnSelectedChanged(Trap newTrap) {
            bool isSelected = newTrap == this._assignedTrap;
            this._colorTween?.Kill();
            if (isSelected) {
                this._colorTween = this._selectionOutline.DOColor(this._selectedColor, 0.4f);
            } else {
                this._colorTween = this._selectionOutline.DOColor(this._defaultColor, 0.1f);
            }
        }

        /// <summary>
        /// Triggers the selection of this trap.
        /// </summary>
        public void SelectTrap() {
            this._selectTrapAction?.Invoke(this._assignedTrap);
        }

        #endregion
    }
}
