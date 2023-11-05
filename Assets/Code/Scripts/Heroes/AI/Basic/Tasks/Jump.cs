using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Jump
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's jump logic. Sets
    /// the hero rigidbody velocity and triggers the jump animation and SFX.
    /// </summary>
    public class Jump : Node {
        private readonly Rigidbody2D _rigidbody;
        private readonly Animator _animController;
        private readonly HeroSoundsController _soundController;
        
        public Jump(Rigidbody2D rigidbody, Animator animController, HeroSoundsController soundController) {
            _rigidbody = rigidbody;
            _animController = animController;
            _soundController = soundController;
        }
        
        /// <summary>
        /// Gets the calculated initial velocity and applies it to the hero to jump. Executes the associated
        /// animations and SFX.
        /// </summary>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            var finalVelocity = (Vector3)GetData("JumpVelocity");
            
            _rigidbody.velocity = finalVelocity;

            _animController.SetTrigger((int)GetData("JumpKey"));
            _soundController.OnHeroJump();
            
            Parent.SetData("JumpLaunchPoint", Vector3.zero);
            
            return NodeStatus.SUCCESS;
        }
    }
}