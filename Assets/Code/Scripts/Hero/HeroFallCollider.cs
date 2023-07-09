using UnityEngine;

public class HeroFallCollider : MonoBehaviour{
  private void OnTriggerEnter2D(Collider2D other) {
    if (!other.CompareTag("Hero")) {
      return;
    }

    Hero hero = other.GetComponent<Hero>();
    hero.Die();
  }
}
