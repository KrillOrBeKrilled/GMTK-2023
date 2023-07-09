using System.Collections;
using Traps;
using UnityEngine;

public class AcidPitTrap : Trap {
  [SerializeField] private int _damageAmount;

  private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
  private Coroutine _intervalDamageCoroutine;

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
    
    hero.HeroMovement.SetSpeedPenalty(0.8f);

    if (this._intervalDamageCoroutine != null) {
      this.StopCoroutine(this._intervalDamageCoroutine);
    }
    this._intervalDamageCoroutine = this.StartCoroutine(this.DealIntervalDamage(hero));
  }

  protected override void OnExitedTrap(Hero hero) {
    if (!IsReady) return;
    
    hero.HeroMovement.ResetSpeedPenalty();
    this.StopCoroutine(this._intervalDamageCoroutine);
  }

  private IEnumerator DealIntervalDamage(Hero hero) {
    while (hero.Health > 0) {
      hero.TakeDamage(this._damageAmount);
      yield return this._waitForOneSecond;
    }

    this._intervalDamageCoroutine = null;
  }
}
