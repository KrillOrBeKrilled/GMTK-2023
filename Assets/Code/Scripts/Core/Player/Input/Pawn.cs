using UnityEngine;

//*******************************************************************************************
// Pawn
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Parent class to handle simple movement methods for controlling a character
    /// adapted to the Command pattern for easy command execution.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : MonoBehaviour {
        [SerializeField] protected float Speed, JumpingForce;
        protected Rigidbody2D RBody;

        private void Awake() {
            this.RBody = this.GetComponent<Rigidbody2D>();
        }

        /// <summary> Sets the character velocity to zero through the <see cref="Rigidbody2D"/>. </summary>
        public void StandIdle() {
            this.RBody.velocity = new Vector2(0f, this.RBody.velocity.y);
        }

        /// <summary>
        /// Sets the character velocity to <see cref="Speed"/> through the <see cref="Rigidbody2D"/>.
        /// </summary>
        /// <param name="direction"> The direction for the character to move; to be multiplied to the speed. </param>
        public virtual void Move(float direction) {
            this.RBody.velocity = new Vector2(direction * this.Speed, this.RBody.velocity.y);
        }

        /// <summary>
        /// Adds a force of <see cref="JumpingForce"/> to the character along the y-axis through the
        /// <see cref="Rigidbody2D"/>.
        /// </summary>
        public virtual void Jump() {
            this.RBody.AddForce(Vector2.up * this.JumpingForce);
        }

        /// <summary> Deploys the equipped trap in the selected tile positions. </summary>
        public virtual void DeployTrap() {
            // Particular to the hen, so will be overrided instead
        }

        /// <summary> Selects or equips a new trap. </summary>
        /// <param name="trapIndex"> The index of the trap to be selected. </param>
        public virtual void ChangeTrap(int trapIndex) {
            // Particular to the hen, so will be overrided instead
        }
    }
}
