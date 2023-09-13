using KrillOrBeKrilled.Common;
using UnityEngine;

//*******************************************************************************************
// FallCollider
//*******************************************************************************************
namespace KrillOrBeKrilled.Environment {
    /// <summary>
    /// Kills any actor that comes in contact with this GameObject.
    /// </summary>
    public class FallCollider : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out IDamageable actor)) {
                actor.Die();
            }
        }
    }
}
