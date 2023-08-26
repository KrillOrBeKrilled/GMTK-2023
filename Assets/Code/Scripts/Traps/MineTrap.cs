using Heroes;
using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {
    [SerializeField] private float _explosionForce = 2f;
    [SerializeField] private int _damageAmount = 10;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + LeftSpawnOffset;
    }
        
    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + RightSpawnOffset;
    }
    
    protected override void SetUpTrap()
    {

    }
    
    protected override void DetonateTrap()
    {

    }

    protected override void OnEnteredTrap(Hero hero) {
      if (!IsReady) return;
      
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.ThrowHeroBack(2f, this._explosionForce);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {

    }
  }
}
