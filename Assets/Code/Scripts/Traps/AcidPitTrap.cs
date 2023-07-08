using System.Collections;
using Traps;
using UnityEngine;

public class AcidPitTrap : Trap {
  [SerializeField] private int _damageAmount;

  private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
  private Coroutine _intervalDamageCoroutine;

  public override void AdjustSpawnPoint() {
    throw new System.NotImplementedException();
  }

  protected override void OnEnteredTrap(Hero hero) {
    hero.HeroMovement.SetSpeedPenalty(0.8f);

    if (this._intervalDamageCoroutine != null) {
      this.StopCoroutine(this._intervalDamageCoroutine);
    }
    this._intervalDamageCoroutine = this.StartCoroutine(this.DealIntervalDamage(hero));
  }

  protected override void OnExitedTrap(Hero hero) {
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
