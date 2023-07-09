using UnityEngine;
using System.Collections.Generic;
using Traps;

public class TrapSelectionBar : MonoBehaviour {
  [SerializeField] private List<TrapBarIcon> _trapBarIcons;

  public void Initialize(Player player) {
    player.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
    CoinManager.Instance.OnCoinAmountChanged.AddListener(this.OnCoinAmountChanged);

    List<Trap> traps = player.PlayerController.Traps;
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
