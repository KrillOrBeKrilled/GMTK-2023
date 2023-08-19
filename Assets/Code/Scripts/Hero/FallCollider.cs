using UnityEngine;

public class FallCollider : MonoBehaviour{
  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Hero")) {
      this.OnHeroTriggerEnter(other.GetComponent<Hero>());
    } else if (other.CompareTag("Player")) {
      this.OnPlayerTriggerEnter(other.GetComponent<Player>());
    }
  }

  private void OnHeroTriggerEnter(Hero hero) {
    if (hero != null)
      hero.Die();
  }

  private void OnPlayerTriggerEnter(Player player) {
    if (player != null)
      player.PlayerController.GameOver();
  }
}
