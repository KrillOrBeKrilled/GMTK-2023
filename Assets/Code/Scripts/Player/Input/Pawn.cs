using UnityEngine;

namespace Player
{
    //*******************************************************************************************
    // Pawn
    //*******************************************************************************************
    /// <summary>
    /// A parent class to handle simple movement methods for controlling a character adapted to
    /// the Command pattern for easy command execution.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : MonoBehaviour
    {
        [SerializeField] protected float Speed, JumpingForce;
        protected Rigidbody2D RBody;

        private void Awake()
        {
            this.RBody = this.GetComponent<Rigidbody2D>();
        }

        public virtual void StandIdle()
        {
            this.RBody.velocity = new Vector2(0f, this.RBody.velocity.y);
        }

        public virtual void Move(float direction)
        {
            this.RBody.velocity = new Vector2(direction * this.Speed, this.RBody.velocity.y);
        }

        public virtual void Jump()
        {
            this.RBody.AddForce(Vector2.up * this.JumpingForce);
        }

        public virtual void FreezePosition() {
            this.RBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public virtual void UnfreezePosition() {
            this.RBody.constraints = RigidbodyConstraints2D.FreezeRotation;
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
