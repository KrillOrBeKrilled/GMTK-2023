using Heroes;
using Player;
using UnityEngine;

public class FallCollider : MonoBehaviour{
  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Hero")) {
      this.OnHeroTriggerEnter(other.GetComponent<Hero>());
    } else if (other.CompareTag("Player")) {
      this.OnPlayerTriggerEnter(other.GetComponent<PlayerManager>());
    }
  }

  private void OnHeroTriggerEnter(Hero hero) {
    if (hero != null)
      hero.Die();
  }

  private void OnPlayerTriggerEnter(PlayerManager playerManager) {
    if (playerManager != null)
      playerManager.PlayerController.GameOver();
  }
}
