using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {
    [SerializeField] private float _explosionForce = 2f;
    [SerializeField] private int _damageAmount = 10;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + _leftSpawnOffset;
    }
        
    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + _rightSpawnOffset;
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
