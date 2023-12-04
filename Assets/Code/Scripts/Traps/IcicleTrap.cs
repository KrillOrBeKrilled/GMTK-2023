using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Managers;
using UnityEngine;

//*******************************************************************************************
// IcicleTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a 2x2 area on the ceiling
    /// and damages the hero when the hero is underneath it.
    /// </summary>
    public class IcicleTrap : Trap {
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;

        private Rigidbody2D _rigidbody2D;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this._rigidbody2D = GetComponent<Rigidbody2D>();
            // Suspend trap in mid-air at the beginning.
            this._rigidbody2D.gravityScale = 0;
        }
        
        /// <summary>
        /// Applies a flat damage to the <see cref="IDamageable"/> hero, and it happens upon collision.
        /// No other special effects at the moment.
        /// </summary>
        protected void OnCollisionEnter2D(Collision2D other) {
            if (!other.gameObject.CompareTag("Hero") ||
                !other.gameObject.TryGetComponent(out IDamageable actor)) {
                return;
            }
                
            actor.TakeDamage(this._damageAmount);
            TilemapManager.Instance.ResetTrapTiles(this.TilePositions);
            Destroy(this.gameObject);
        }
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Give this trap a gravity scale so that it starts falling. </remarks>
        protected override void DetonateTrap() {
            this._rigidbody2D.gravityScale = 3;
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap starts falling when it detects an <see cref="IDamageable"/>
        /// instance (e.g. a hero).
        /// </summary>
        protected override void OnEnteredTrap(IDamageable actor) {
            if (!this.IsReady) {
                return;
            }

            DetonateTrap();
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary> No responses for exiting the trap. </summary>
        protected override void OnExitedTrap(IDamageable actor) {}
        
        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> No additional responses at the moment. </remarks>
        protected override void SetUpTrap() {}

        #endregion
    }
}