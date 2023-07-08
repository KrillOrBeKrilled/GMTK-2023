using UnityEngine;

namespace Traps {
  public class SpikeTrap : Trap {
    [SerializeField] private int _damageAmount;

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected  override void TriggerTrap(Hero hero) {
      print("Hit spikes!");
      hero.TakeDamage(this._damageAmount);
    }
  }
}
