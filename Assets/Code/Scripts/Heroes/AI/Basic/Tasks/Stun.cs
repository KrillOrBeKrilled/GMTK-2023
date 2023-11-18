using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Stun
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's jump logic. Sets
    /// the hero rigidbody velocity and triggers the jump animation and SFX.
    /// </summary>
    public class Stun : Node {
        private float _stunTimer;
        
        /// <summary>
        /// Gets the calculated initial velocity and applies it to the hero to jump. Executes the associated
        /// animations and SFX.
        /// </summary>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            if (!(bool)GetData("IsStunned")) {
                return NodeStatus.FAILURE;
            }
            
            if (_stunTimer < (float)GetData("StunDuration")) {
                _stunTimer += Time.deltaTime;
                
                Parent.SetData("IsStunned", true);
                return NodeStatus.RUNNING;
            }

            _stunTimer = 0f;
            Parent.SetData("IsStunned", false);

            return NodeStatus.SUCCESS;
        }
    }
}