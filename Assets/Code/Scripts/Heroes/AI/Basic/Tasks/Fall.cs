using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Fall
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class Fall : Node {
        private Transform _heroTransform;
        private Rigidbody2D _rigidbody;
        private Animator _animController;
        private readonly LayerMask _groundLayers;

        public Fall(Transform heroTransform, Rigidbody2D rigidbody, LayerMask groundLayers) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _groundLayers = groundLayers;
        }
        
        internal override NodeStatus Evaluate() {
            var heroPos = _heroTransform.position;
            var velocity = _rigidbody.velocity;
            
            // Check if the hero is currently on the ground
            var hit = Physics2D.Raycast(heroPos, Vector2.down, 2f, _groundLayers);
            
            if (velocity.y > 0.1f || !hit) {
                var landPos = (Vector3)GetData("JumpLandPoint");
                
                // Dampen hero movement past the target land point to make more accurate jumps
                if (landPos != Vector3.zero && velocity.x > 0.04f && heroPos.x > landPos.x) {
                    velocity.x -= 0.04f;
                    _rigidbody.velocity = velocity;
                }
                
                Parent.Parent.SetData("IsFalling", true);
                return NodeStatus.SUCCESS;
            }
            
            Parent.Parent.SetData("IsFalling", false);
            return NodeStatus.FAILURE;
        }
    }
}