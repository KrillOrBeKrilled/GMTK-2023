using DG.Tweening;
using KrillOrBeKrilled.Model;
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
        
        private bool _isArmed = false;
        private bool _firstSetup = true;
        
        
        //========================================
        // Public Methods
        //======================================== 
        
        public override TrapType GetTrapType() {
            return TrapType.Spikes;
        }
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        protected void ArmTrap() {
            this._isArmed = true;
        }
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Immediately juts the spikes out when the hero walks over them. </remarks>
        protected override void DetonateTrap() {
            this._isArmed = false;
            this.transform
                .DOMove(this.SpawnPosition + this.AnimationOffset, 0.05f)
                .OnComplete(this.SetUpTrap);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// Applies a 30% speed reduction and flat damage to the <see cref="ITrapDamageable"/> actor.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected  override void OnEnteredTrap(ITrapDamageable actor) {
            if (!this._isArmed)
                return;
            
            DetonateTrap();
            actor.TakeTrapDamage(this._damageAmount, this);
            actor.ApplyTrapSpeedPenalty(0.3f);
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary> Resets the speed reduction acting on the <see cref="ITrapDamageable"/> actor. </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnExitedTrap(ITrapDamageable actor) {
            actor.ResetTrapSpeedPenalty();
        }
        
        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks>
        /// Pulls the spikes into the ground in wait of the <see cref="ITrapDamageable"/> actor to walk over them.
        /// </remarks>
        protected override void SetUpTrap() {
            float duration = this._firstSetup ? 1f : 4f;
            if (this._firstSetup)
                this._firstSetup = false;

            this.transform
                .DOMove(this.SpawnPosition + Vector3.down * 0.2f, duration)
                .OnComplete(this.ArmTrap);
        }
        
        #endregion
    }
}
