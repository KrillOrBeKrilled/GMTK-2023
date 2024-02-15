using KrillOrBeKrilled.Interfaces;
using DG.Tweening;
using KrillOrBeKrilled.Traps.Interfaces;
using UnityEngine;

//*******************************************************************************************
// SpikeTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a permanent 3x1 grounded area and
    /// damages the hero with a speed penalty. 
    /// </summary>
    public class SpikeTrap : Trap {
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Immediately juts the spikes out when the hero walks over them. </remarks>
        protected override void DetonateTrap() {
            this.transform.DOComplete();
            this.transform.DOMove(this.SpawnPosition + this.AnimationOffset, 0.05f);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// Applies a 30% speed reduction and flat damage to the <see cref="ITrapDamageable"/> actor.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected  override void OnEnteredTrap(ITrapDamageable actor) {
            if (!this.IsReady) {
                return;
            }

            DetonateTrap();
            actor.TakeDamage(this._damageAmount, this);
            actor.ApplySpeedPenalty(0.3f);
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary> Resets the speed reduction acting on the <see cref="ITrapDamageable"/> actor. </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnExitedTrap(ITrapDamageable actor) {
            if (!this.IsReady) {
                return;
            }

            actor.ResetSpeedPenalty();
        }
        
        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks>
        /// Pulls the spikes into the ground in wait of the <see cref="ITrapDamageable"/> actor to walk over them.
        /// </remarks>
        protected override void SetUpTrap() {
            transform.DOMove(this.SpawnPosition + Vector3.down * 0.2f, 1f);
        }
        
        #endregion
    }
}
