//*******************************************************************************************
// IDamageable
//*******************************************************************************************
namespace KrillOrBeKrilled.Common {
    /// <summary>
    /// Used to declutter the PlayerController class, encapsulating the player's
    /// behaviour in each state to better reason about correctness and pinpoint bugs
    /// easily, plus specialize behaviours to specific states.
    /// </summary>
    public interface IDamageable {
        /// <summary> Deals damage to the actor. </summary>
        /// <param name="amount"> The value to subtract from the actor's health. </param>
        public void TakeDamage(int amount);

        public void ThrowActorBack(float stunDuration, float throwForce);

        public void ApplySpeedPenalty(float penalty);

        public void ResetSpeedPenalty();
    
        /// <summary>
        /// Defeats the actor, disabling all controls and playing associated animations before destroying the actor.
        /// </summary>
        public void Die();
    }
}

