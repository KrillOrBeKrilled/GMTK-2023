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
        private readonly Animator _animController;
        
        private float _stunTimer;
        
        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>.
        /// </summary>
        /// <param name="animController"> Used to animate the hero when stunned. </param>
        public Stun(Animator animController) {
            this._animController = animController;
        }
        
        /// <summary>
        /// Gets the calculated initial velocity and applies it to the hero to jump. Executes the associated
        /// animations and SFX.
        /// </summary>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            if (!(bool)GetData("IsStunned")) {
                return NodeStatus.FAILURE;
            }
            
            if (this._stunTimer < (float)GetData("StunDuration")) {
                this._stunTimer += Time.deltaTime;
                
                Parent.SetData("IsStunned", true);
                this._animController.SetBool((int)GetData("StunnedKey"), true);
                
                return NodeStatus.RUNNING;
            }

            this._stunTimer = 0f;
            Parent.SetData("IsStunned", false);
            this._animController.SetBool((int)GetData("StunnedKey"), false);

            return NodeStatus.SUCCESS;
        }
    }
}