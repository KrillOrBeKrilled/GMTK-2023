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
            var jumpForce = (float)GetData("JumpForce");
            var finalVelocity = (Vector3)GetData("JumpAngle");
            
            _rigidbody.velocity = finalVelocity;

            _animController.SetTrigger((int)GetData("JumpKey"));
            _soundController.OnHeroJump();
            
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");
            
            Parent.SetData("JumpLaunchPoint", Vector3.zero);
            Parent.SetData("JumpLandPoint", Vector3.zero);
            Parent.SetData("JumpInitialMinHeight", Vector3.zero);
            Parent.SetData("JumpApexMinHeight", Vector3.zero);
            
            return NodeStatus.SUCCESS;
        }
    }
}