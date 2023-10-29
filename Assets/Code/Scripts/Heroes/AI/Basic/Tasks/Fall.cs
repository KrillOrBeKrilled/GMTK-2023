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
            // Check if the hero is currently on the ground
            var hit = Physics2D.Raycast(_heroTransform.position, Vector2.down, 2f, _groundLayers);
            
            if (_rigidbody.velocity.y > 0.01f || !hit) {
                return NodeStatus.SUCCESS;
            }
            
            // Hero is touching the ground
            // Parent.Parent.SetData("CanJump", true);
            
            return NodeStatus.FAILURE;
        }
    }
}