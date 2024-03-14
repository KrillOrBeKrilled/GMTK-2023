using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Stun
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's stunned status.
    /// Manages the hero animations and SFX when stunned.
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
        /// Prevents hero movement until the stun duration has passed.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if the stun duration has passed and the hero is leaving the stunned state.
        /// The <b>running</b> status if the hero is stunned and currently awaiting the stun duration.
        /// The <b>failure</b> status if the hero is not stunned.
        /// </returns>
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