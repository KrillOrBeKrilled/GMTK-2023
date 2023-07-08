using UnityEngine;

namespace Traps {
  public class SwingingAxeTrap : Trap {
    [SerializeField] private float _pushbackForce = 1f;
    [SerializeField] private int _damageAmount = 10;

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected override void OnEnteredTrap(Hero hero) {
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.ThrowHeroBack(0.5f, this._pushbackForce);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {
      throw new System.NotImplementedException();
    }
  }
}
