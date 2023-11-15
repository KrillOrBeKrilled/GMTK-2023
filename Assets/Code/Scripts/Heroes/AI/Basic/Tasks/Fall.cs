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

        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>.
        /// </summary>
        /// <param name="heroTransform"> Provides the position of the hero. </param>
        /// <param name="rigidbody"> Drives the hero animation when falling and controls midair velocity. </param>
        /// <param name="animController"> Used to animate the hero when falling midair. </param>
        /// <param name="groundLayers"> The LayerMask used to check for when the hero is grounded. </param>
        public Fall(Transform heroTransform, Rigidbody2D rigidbody, Animator animController, LayerMask groundLayers) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _animController = animController;
            _groundLayers = groundLayers;
        }
        
        /// <summary>
        /// Checks when the hero is midair or grounded and updates its status accordingly.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if the hero is currently midair and falling.
        /// The <b>failure</b> status if the hero is grounded.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var heroPos = _heroTransform.position;
            var velocity = _rigidbody.velocity;
            
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            
            // Check if the hero is currently midair, not touching the ground
            if (!Physics2D.Raycast(heroPos, Vector2.down, 2f, _groundLayers) 
                || velocity.y > 0.1f) {
                var landPos = (Vector3)GetData("JumpLandPoint");
                
                // Dampen hero movement past the target land point to make more accurate jumps
                if (landPos != Vector3.zero && velocity.x > 0.01f && heroPos.x > landPos.x) {
                    velocity.x -= 0.005f;
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