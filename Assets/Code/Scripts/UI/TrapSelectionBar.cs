using UnityEngine;
using System.Collections.Generic;

public class TrapSelectionBar : MonoBehaviour {
  [SerializeField] private List<TrapBarIcon> _trapBarIcons;

  public void Initialize(Player player) {
    player.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
  }

  private void SelectedTrapIndexChanged(int newIndex) {
    for (int i = 0; i < this._trapBarIcons.Count; i++) {
      this._trapBarIcons[i].OnSelectedChanged(i == newIndex);
    }
  }
}
