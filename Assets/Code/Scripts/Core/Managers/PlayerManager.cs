using KrillOrBeKrilled.Core.Input;
using KrillOrBeKrilled.Player;
using UnityEngine;

//*******************************************************************************************
// PlayerManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Manages the player GameObject as an entity that interacts with other GameObjects
    /// in the environment and acts as an intermediary to grant access to the
    /// <see cref="PlayerCharacter"/> and <see cref="TrapController"/> to other classes.
    /// </summary>
    public class PlayerManager : MonoBehaviour {
        public PlayerController PlayerController;
        
        [Tooltip("The PlayerController associated with the player GameObject.")]
        internal PlayerCharacter Player { get; private set; }
        
        [Tooltip("The TrapController associated with the player GameObject.")]
        internal TrapController TrapController { get; private set; }

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this.Player = this.GetComponent<PlayerCharacter>();
            this.TrapController = this.GetComponent<TrapController>();
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hero")) {
                return;
            }

            this.Player.Die();
        }
        
        #endregion
    }
}
