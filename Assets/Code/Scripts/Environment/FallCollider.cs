using KrillOrBeKrilled.Interfaces;
using UnityEngine;

//*******************************************************************************************
// FallCollider
//*******************************************************************************************
namespace KrillOrBeKrilled.Environment {
    /// <summary>
    /// Kills any actor that comes in contact with this GameObject.
    /// </summary>
    public class FallCollider : MonoBehaviour {

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Start() {
            this.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent(out IDamageable actor)) {
                actor.Die();
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
                Destroy(other.gameObject);
            }
        }

        #endregion
    }
}
