using System.Collections.Generic;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Traps;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// TrapSelectionBar
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages the initialization and updates to the
    /// <see cref="TrapBarIcon">TrapBarIcons</see> upon updates to the trap and coin
    /// management systems.
    /// </summary>
    public class TrapSelectionBar : MonoBehaviour {
        [Tooltip("Stores each trap icon contained within the trap selection toolbar.")]
        [SerializeField] private List<TrapBarIcon> _trapBarIcons;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Sets up all references and listeners to operate the trap selection toolbar, also initializing each
        /// <see cref="TrapBarIcon"/>.
        /// </summary>
        /// <param name="trapChanged">Event triggered when selected trap index was updated. </param>
        /// <param name="traps">A list of all traps. </param>
        /// <param name="selectTrapAction"> A callback which is invoked when a Trap Icon is clicked. </param>
        public void Initialize(UnityEvent<Trap> trapChanged, ReadOnlyCollection<Trap> traps, UnityAction<Trap> selectTrapAction) {
            trapChanged.AddListener(this.SelectedTrapIndexChanged);
            EventManager.Instance.ResourceAmountChangedEvent.AddListener(this.OnResourceAmountChanged);

            for (int i = 0; i < this._trapBarIcons.Count; i++) {
                this._trapBarIcons[i].Initialize(traps[i], selectTrapAction);
            }
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        // /// <summary>
        // /// Updates each <see cref="TrapBarIcon"/> tint when the coin balance updates.
        // /// </summary>
        // /// <param name="newAmount"> The current number of coins available from the <see cref="CoinManager"/>. </param>
        // /// <remarks> Listens on the <see cref="CoinAmountChangedEvent"/> event. </remarks>
        // private void OnCoinAmountChanged(int newAmount) {
        //     this._trapBarIcons.ForEach(trapBarIcon => trapBarIcon.OnCanAffordChanged(newAmount));
        // }
        
        /// <summary>
        /// Updates each <see cref="TrapBarIcon"/> tint when the resource inventory updates.
        /// </summary>
        /// <remarks> Listens on the <see cref="ResourceAmountChangedEvent"/> event. </remarks>
        private void OnResourceAmountChanged(ResourceType type, int amount) {
            this._trapBarIcons.ForEach(trapBarIcon => trapBarIcon.CheckAffordable());
        }

        /// <summary>
        /// Updates each <see cref="TrapBarIcon"/> when a new trap is selected.
        /// </summary>
        /// <param name="newTrap"> The newly selected trap. </param>
        /// <remarks> Listens on the trapChanged event provided in <see cref="Initialize"/>. </remarks>
        private void SelectedTrapIndexChanged(Trap newTrap) {
            foreach (TrapBarIcon trapIcon in this._trapBarIcons) {
                trapIcon.OnSelectedChanged(newTrap);
            }
        }

        #endregion
    }
}
