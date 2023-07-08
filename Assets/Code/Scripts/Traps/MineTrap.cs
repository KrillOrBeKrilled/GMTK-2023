using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {
    [SerializeField] private float _explosionForce = 2f;
    [SerializeField] private int _damageAmount = 10;

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected override void OnEnteredTrap(Hero hero) {
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.ThrowHeroBack(2f, this._explosionForce);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {

    }
  }
}
