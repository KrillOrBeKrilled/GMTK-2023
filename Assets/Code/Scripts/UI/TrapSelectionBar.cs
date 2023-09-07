using Managers;
using Player;
using System.Collections.Generic;
using Traps;
using UnityEngine;

//*******************************************************************************************
// TrapSelectionBar
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Manages the initialization and updates to the
    /// <see cref="TrapBarIcon">TrapBarIcons</see> upon updates to the trap and coin
    /// management systems.
    /// </summary>
    public class TrapSelectionBar : MonoBehaviour {
        [Tooltip("Stores each trap icon contained within the trap selection toolbar.")]
        [SerializeField] private List<TrapBarIcon> _trapBarIcons;

        /// <summary>
        /// Sets up all references and listeners to operate the trap selection toolbar, also initializing each
        /// <see cref="TrapBarIcon"/>.
        /// </summary>
        /// <param name="playerManager"> Provides event and data references related to the trap system to subscribe
        /// to and link trap icons. </param>
        public void Initialize(PlayerManager playerManager) {
            playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
            CoinManager.Instance.OnCoinAmountChanged.AddListener(this.OnCoinAmountChanged);

            List<Trap> traps = playerManager.TrapController.Traps;
            for (int i = 0; i < this._trapBarIcons.Count; i++) {
                this._trapBarIcons[i].Initialize(traps[i]);
            }
        }

        /// <summary> Updates each <see cref="TrapBarIcon"/> when a new trap is selected. </summary>
        /// <param name="newIndex"> The index of the current selected trap. </param>
        /// <remarks> Listens on the <see cref="PlayerController.OnSelectedTrapIndexChanged"/> event. </remarks>
        private void SelectedTrapIndexChanged(int newIndex) {
            for (int i = 0; i < this._trapBarIcons.Count; i++) {
                this._trapBarIcons[i].OnSelectedChanged(i == newIndex);
            }
        }

        /// <summary> Updates each <see cref="TrapBarIcon"/> tint when the coin balance updates. </summary>
        /// <param name="newAmount"> The current number of coins available from the <see cref="CoinManager"/>. </param>
        /// <remarks> Listens on the <see cref="CoinManager.OnCoinAmountChanged"/> event. </remarks>
        private void OnCoinAmountChanged(int newAmount) {
            this._trapBarIcons.ForEach(trapBarIcon => trapBarIcon.OnCanAffordChanged(newAmount));
        }
    }
}
