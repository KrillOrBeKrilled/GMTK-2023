using UnityEngine;

//*******************************************************************************************
// PlayerManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Manages the player GameObject as an entity that interacts with other GameObjects
    /// in the environment and acts as an intermediary to grant access to the
    /// <see cref="PlayerController"/> and <see cref="TrapController"/> to other classes.
    /// </summary>
    public class PlayerManager : MonoBehaviour {
        [Tooltip("The PlayerController associated with the player GameObject.")]
        internal PlayerController PlayerController { get; private set; }
        [Tooltip("The TrapController associated with the player GameObject.")]
        internal TrapController TrapController { get; private set; }

        private void Awake() {
            this.PlayerController = this.GetComponent<PlayerController>();
            this.TrapController = this.GetComponent<TrapController>();
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hero")) {
                return;
            }

            this.PlayerController.Die();
        }
    }
}
