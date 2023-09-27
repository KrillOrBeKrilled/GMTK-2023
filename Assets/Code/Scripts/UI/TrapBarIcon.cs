using DG.Tweening;
using KrillOrBeKrilled.Traps;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public class TrapBarIcon : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private Image _selectionOutline;
        [SerializeField] private Image _tint;
        [SerializeField] private Image _icon;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;

        private UnityAction<Trap> _selectTrapAction;
        private Trap _assignedTrap;
        
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
        
        /// <summary>
        /// Adds a tint to this icon if the corresponding trap is not affordable.
        /// </summary>
        /// <param name="newAmount"> The current number of coins available. </param>
        public void OnCanAffordChanged(int newAmount) {
            this._tint.gameObject.SetActive(newAmount < this._assignedTrap.Cost);
        }
        
        /// <summary>
        /// Triggered by Unity when this icon is clicked.
        /// </summary>
        /// <param name="eventData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnPointerClick(PointerEventData eventData) {
            this._selectTrapAction?.Invoke(this._assignedTrap);
        }

        /// <summary>
        /// Outlines this icon if the corresponding trap is currently selected.
        /// </summary>
        /// <param name="newTrap">The newly selected trap.</param>
        public void OnSelectedChanged(Trap newTrap) {
            bool isSelected = newTrap == this._assignedTrap;
            Color targetColor = isSelected ? this._selectedColor : this._defaultColor;
            this._selectionOutline.DOColor(targetColor, 0.3f);

            if (isSelected) {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(this._icon.rectTransform.DOScale(new Vector3(0.92f, 0.92f, 1f), 0.3f));
                sequence.Append(this._icon.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f));
            }
        }
        
        #endregion
    }
}
