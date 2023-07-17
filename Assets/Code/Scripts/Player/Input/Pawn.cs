using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input
{
    // A class to be utilized for all possible movement of a playable character that will be executed by respective
    // commands
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : MonoBehaviour
    {
        [SerializeField] protected float Speed, JumpingForce;
        protected Rigidbody2D RBody;

        private void Awake()
        {
            Debug.Log("Pawn awake called");
            RBody = GetComponent<Rigidbody2D>();
        }
        
        public virtual void StandIdle()
        {
            RBody.velocity = new Vector2(0f, RBody.velocity.y);
        }
        
        public virtual void Move(float direction)
        {
            RBody.velocity = new Vector2(direction * Speed, RBody.velocity.y);
        }

        public virtual void Jump()
        {
            RBody.AddForce(Vector2.up * JumpingForce);
        }

        public virtual void DeployTrap()
        {
            // Particular to the hen, so will be overrided instead
        }
        
        public virtual void ChangeTrap(int trapIndex)
        {
            // Particular to the hen, so will be overrided instead
        }
    }
}

