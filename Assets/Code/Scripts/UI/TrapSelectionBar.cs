using Managers;
using Player;
using System.Collections.Generic;
using Traps;
using UnityEngine;

namespace UI {
  public class TrapSelectionBar : MonoBehaviour {
    [SerializeField] private List<TrapBarIcon> _trapBarIcons;

    public void Initialize(PlayerManager playerManager) {
      playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
      CoinManager.Instance.OnCoinAmountChanged.AddListener(this.OnCoinAmountChanged);

      List<Trap> traps = playerManager.TrapController.Traps;
      for (int i = 0; i < this._trapBarIcons.Count; i++) {
        this._trapBarIcons[i].Initialize(traps[i]);
      }
    }

    private void SelectedTrapIndexChanged(int newIndex) {
      for (int i = 0; i < this._trapBarIcons.Count; i++) {
        this._trapBarIcons[i].OnSelectedChanged(i == newIndex);
      }
    }

    private void OnCoinAmountChanged(int newAmount) {
      this._trapBarIcons.ForEach(trapBarIcon => trapBarIcon.OnCanAffordChanged(newAmount));
    }
  }
}
