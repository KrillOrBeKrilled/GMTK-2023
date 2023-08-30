using Heroes;
using Player;
using UnityEngine;

//*******************************************************************************************
// FallCollider
//*******************************************************************************************
/// <summary>
/// Ends the game upon triggered collisions with the player and causes hero death
/// upon contact with the hero.
/// </summary>
public class FallCollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Hero")) {
          this.OnHeroTriggerEnter(other.GetComponent<Hero>());
        } else if (other.CompareTag("Player")) {
          this.OnPlayerTriggerEnter(other.GetComponent<PlayerManager>());
        }
    }

    /// <summary>
    /// Triggers hero death through <see cref="Hero"/>.
    /// </summary>
    /// <param name="hero"> Used to invoke <see cref="Hero.Die"/> for the hero. </param>
    private void OnHeroTriggerEnter(Hero hero) {
        if (hero != null) 
            hero.Die();
    }

    /// <summary>
    /// Triggers game over through the <see cref="PlayerManager"/>.
    /// </summary>
    /// <param name="playerManager"> Used to invoke <see cref="PlayerController.GameOver"/>. </param>
    private void OnPlayerTriggerEnter(PlayerManager playerManager) {
      if (playerManager != null) 
          playerManager.PlayerController.GameOver();
    }
}
