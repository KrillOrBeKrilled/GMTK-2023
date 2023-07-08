using UnityEngine;

namespace Traps {
  public class SpikeTrap : Trap {
    [SerializeField] private int _damageAmount;

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected  override void OnEnteredTrap(Hero hero) {
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.SetSpeedPenalty(0.8f);
    }

    protected override void OnExitedTrap(Hero hero) {
      hero.HeroMovement.ResetSpeedPenalty();
    }
  }
}
