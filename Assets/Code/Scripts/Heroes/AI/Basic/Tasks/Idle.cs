using Codice.Client.BaseCommands;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Idle
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class Idle : Node {
        private Rigidbody2D _rigidbody;
        private Animator _animController;

        public Idle(Rigidbody2D rigidbody, Animator animController) {
            _rigidbody = rigidbody;
            _animController = animController;
        }
        
        internal override NodeStatus Evaluate() {
            Debug.Log("Idle");
            this._rigidbody.velocity = Vector2.zero;
            
            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            return NodeStatus.SUCCESS;
        }
    }
}
