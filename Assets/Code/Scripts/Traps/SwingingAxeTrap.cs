using UnityEngine;

namespace Traps {
  public class SwingingAxeTrap : Trap {
    [SerializeField] private float _pushbackForce = 1f;
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
      hero.HeroMovement.ThrowHeroBack(0.5f, this._pushbackForce);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {
      throw new System.NotImplementedException();
    }
  }
}
