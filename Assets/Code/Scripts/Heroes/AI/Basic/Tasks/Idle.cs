using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Idle
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's ability to stay
    /// idle.
    /// </summary>
    public class Idle : Node {
        private Rigidbody2D _rigidbody;
        private Animator _animController;

        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>.
        /// </summary>
        /// <param name="rigidbody"> Drives the hero movement animation. </param>
        /// <param name="animController"> Used to animate the hero during regular movement. </param>
        public Idle(Rigidbody2D rigidbody, Animator animController) {
            _rigidbody = rigidbody;
            _animController = animController;
        }
        
        /// <summary>
        /// Clears the hero's velocity and updates the animation controller accordingly.
        /// </summary>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            this._rigidbody.velocity = Vector2.zero;
            
            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            return NodeStatus.SUCCESS;
        }
    }
}
