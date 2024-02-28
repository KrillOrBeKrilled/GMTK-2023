//*******************************************************************************************
// IDamageable
//*******************************************************************************************
namespace KrillOrBeKrilled.Interfaces {
    /// <summary>
    /// Interfaces any actor that can be damaged by traps and/or environmental factors.
    /// Encapsulates all logic associated with actors taking damage, debuffs, and dying.
    /// </summary>
    public interface IDamageable {
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Defeats the actor, disabling all controls and playing associated animations before destroying the actor.
        /// </summary>
        public void Die();
        
        /// <summary>
        /// Deals damage to the actor.
        /// </summary>
        /// <param name="amount"> The value to subtract from the actor's health. </param>
        public void TakeDamage(int amount);
        
        /// <summary>
        /// Applies a force to knock back and stun the actor.
        /// </summary>
        /// <param name="stunDuration"> The duration of time to stun the actor. </param>
        /// <param name="throwForce"> Scales the knock back force applied to the actor. </param>
        public void ThrowActorBack(float stunDuration, float throwForce);

        #endregion
    }
}

