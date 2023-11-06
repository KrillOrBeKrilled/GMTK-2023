//*******************************************************************************************
// IDamageable
//*******************************************************************************************
namespace KrillOrBeKrilled.Common.Interfaces {
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
        /// Reduces the actor's movement speed by a percentage reduction.
        /// </summary>
        /// <param name="penalty"> The speed penalty value to limit the actor's movement. </param>
        /// <remarks> The incremented speed penalty is clamped between [0,1] as a percentage value. </remarks>
        public void ApplySpeedPenalty(float penalty);
        
        /// <summary>
        /// Defeats the actor, disabling all controls and playing associated animations before destroying the actor.
        /// </summary>
        public void Die();
        
        /// <summary>
        /// Gets the actor's current health.
        /// </summary>
        public int GetHealth();
        
        /// <summary>
        /// Resets the speed penalty to return the actor movement speed to normal.
        /// </summary>
        public void ResetSpeedPenalty();
        
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
        
        /// <summary>
        /// Applies a force to make the actor leap forward.
        /// </summary>
        /// <param name="throwForce"> Scales the force applied to the actor. </param>
        public void ThrowActorForward(float throwForce);

        #endregion
    }
}

