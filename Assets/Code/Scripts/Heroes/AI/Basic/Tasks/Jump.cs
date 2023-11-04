using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Jump
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class Jump : Node {
        private Rigidbody2D _rigidbody;
        private Animator _animController;
        private HeroSoundsController _soundController;
        
        public Jump(Rigidbody2D rigidbody, Animator animController, HeroSoundsController soundController) {
            _rigidbody = rigidbody;
            _animController = animController;
            _soundController = soundController;
        }
        
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