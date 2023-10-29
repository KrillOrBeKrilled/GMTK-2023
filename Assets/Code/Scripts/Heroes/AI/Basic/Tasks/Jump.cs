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
            Debug.Log("Jump force: " + jumpForce);
            
            _rigidbody.AddForce(Vector2.up * (jumpForce * _rigidbody.mass * _rigidbody.gravityScale), ForceMode2D.Impulse);

            _animController.SetTrigger((int)GetData("JumpKey"));
            _soundController.OnHeroJump();
            
            var pitQueue = (Queue<(Vector3, Vector3)>)GetData("PitQueue");
            
            // TODO: Dequeue here for now but probably dequeue this when the jump is complete next
            // Jump will be complete, no matter what so do all the dequeue stuff here
            pitQueue.Dequeue();
            
            // Parent.Parent.Parent.SetData("CanJump", false);
            Parent.SetData("JumpLaunchPoint", Vector3.zero);
            Parent.SetData("JumpLandPoint", Vector3.zero);
            Parent.SetData("JumpInitialMinHeight", Vector3.zero);
            Parent.SetData("JumpApexMinHeight", Vector3.zero);
            
            return NodeStatus.SUCCESS;
        }
    }
}