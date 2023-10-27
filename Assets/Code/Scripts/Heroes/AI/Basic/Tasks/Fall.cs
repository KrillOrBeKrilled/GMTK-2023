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
            if (_rigidbody.velocity.y > 0.01f || !Physics2D.Raycast(_heroTransform.position, Vector2.down, 2f, _groundLayers)) {
                return NodeStatus.SUCCESS;
            }

            // If the y velocity is near zero, then the hero is not falling
            Parent.SetData("CanJump", true);
            
            Debug.Log("Set CanJump to true!");
            return NodeStatus.FAILURE;
        }
    }
}