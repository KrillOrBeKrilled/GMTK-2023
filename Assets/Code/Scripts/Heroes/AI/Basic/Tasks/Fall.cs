using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Fall
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's actions while
    /// midair.
    /// </summary>
    public class Fall : Node {
        private readonly Transform _heroTransform;
        private readonly Rigidbody2D _rigidbody;
        private readonly Animator _animController;
        private readonly LayerMask _groundLayers;

        public Fall(Transform heroTransform, Rigidbody2D rigidbody, Animator animController, LayerMask groundLayers) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _animController = animController;
            _groundLayers = groundLayers;
        }
        
        /// <summary>
        /// Checks when the hero is midair or grounded and updates its status accordingly. When midair, slows the
        /// hero's horizontal movement if it has passed the target land position to increase jump accuracy.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if the hero is currently midair and falling.
        /// The <b>failure</b> status if the hero is grounded.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var heroPos = _heroTransform.position;
            var velocity = _rigidbody.velocity;
            
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            
            // Check if the hero is currently on the ground
            var hit = Physics2D.Raycast(heroPos, Vector2.down, 2f, _groundLayers);
            
            if (!hit || velocity.y > 0.1f) {
                Parent.Parent.SetData("IsFalling", true);
                return NodeStatus.SUCCESS;
            }
            
            Parent.Parent.SetData("IsFalling", false);
            return NodeStatus.FAILURE;
        }
    }
}