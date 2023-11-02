using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Run
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class Run : Node {
        private Rigidbody2D _rigidbody;
        private Animator _animController;

        private float _movementSpeed;

        public Run(Rigidbody2D rigidbody, Animator animController, float movementSpeed) {
            _rigidbody = rigidbody;
            _animController = animController;

            _movementSpeed = movementSpeed;
        }
        
        internal override NodeStatus Evaluate() {
            if (!(bool)GetData("IsMoving")) {
                return NodeStatus.FAILURE;
            }
            
            var speed = this._movementSpeed * (1f - (float)GetData("SpeedPenalty"));
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));

            return NodeStatus.SUCCESS;
        }
    }
}

