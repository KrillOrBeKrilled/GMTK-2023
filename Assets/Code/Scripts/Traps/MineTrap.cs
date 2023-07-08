using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {
    [SerializeField] private float _explosionForce = 2f;
    [SerializeField] private int _damageAmount = 10;

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected override void OnEnteredTrap(Hero hero) {
      hero.HeroMovement.Stun(2f);
      hero.TakeDamage(this._damageAmount);

      Rigidbody2D heroRigidbody2D = hero.GetComponent<Rigidbody2D>();

      Vector2 explosionVector = new Vector2(-1f, 0.7f) * this._explosionForce;
      heroRigidbody2D.AddForce(explosionVector, ForceMode2D.Impulse);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {

    }
  }
}
