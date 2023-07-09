using UnityEngine;

namespace Traps {
  public class SpikeTrap : Trap {
    [SerializeField] private int _damageAmount;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + _leftSpawnOffset;
    }
        
    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + _rightSpawnOffset;
    }

    protected  override void OnEnteredTrap(Hero hero) {
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.SetSpeedPenalty(0.3f);
    }

    protected override void OnExitedTrap(Hero hero) {
      hero.HeroMovement.ResetSpeedPenalty();
    }
  }
}
