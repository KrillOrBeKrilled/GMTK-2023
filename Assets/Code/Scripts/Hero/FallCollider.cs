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
    if (hero is not null)
      hero.Die();
  }

  private void OnPlayerTriggerEnter(Player player) {
    if (player is not null)
      player.PlayerController.GameOver();
  }
}
