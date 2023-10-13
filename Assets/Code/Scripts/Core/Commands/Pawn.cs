using KrillOrBeKrilled.Traps;
using UnityEngine;

//*******************************************************************************************
// Pawn
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Commands {
    /// <summary>
    /// Parent class to handle simple movement methods for controlling a character
    /// adapted to the Command pattern for easy command execution.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : MonoBehaviour {
        [SerializeField] protected float Speed, JumpingForce, GlideVelocityLimit;
        protected Rigidbody2D RBody;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            this.RBody = this.GetComponent<Rigidbody2D>();
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Selects or equips a new trap.
        /// </summary>
        /// <param name="trap"> The trap to be selected. </param>
        public virtual void ChangeTrap(Trap trap) {
            // Particular to the hen, so will be overrided instead
        }

        /// <summary>
        /// Deploys the equipped trap in the selected tile positions.
        /// </summary>
        public virtual void DeployTrap() {
            // Particular to the hen, so will be overrided instead
        }

        /// <summary>
        /// Freezes the character position through the <see cref="Rigidbody2D"/>.
        /// </summary>
        public virtual void FreezePosition() {
            this.RBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        /// <summary>
        /// Adds a force of <see cref="JumpingForce"/> to the character along the y-axis through the
        /// <see cref="Rigidbody2D"/>.
        /// </summary>
        /// <param name="forceMultiplier">The multiplier to apply to the force</param>
        public virtual void Jump(float forceMultiplier) {
            this.RBody.AddForce(this.JumpingForce * forceMultiplier * Vector2.up);
        }

        /// <summary>
        /// Sets the character velocity to <see cref="Speed"/> through the <see cref="Rigidbody2D"/>.
        /// </summary>
        /// <param name="moveInput"> The move input value for the character to move by; to be multiplied to the speed. </param>
        public virtual void Move(float moveInput) {
            this.RBody.velocity = new Vector2(moveInput * this.Speed, this.RBody.velocity.y);
        }

        /// <summary>
        /// Sets the character velocity to <see cref="Speed"/> through the <see cref="Rigidbody2D"/>.
        /// </summary>
        /// <param name="moveInput"> The move input value for the character to move by; to be multiplied to the speed. </param>
        public virtual void Glide(float moveInput) {
            float clampedYVelocity = Mathf.Max(this.RBody.velocity.y, this.GlideVelocityLimit);
            this.RBody.velocity = new Vector2(moveInput * this.Speed, clampedYVelocity);
            print(this.RBody.velocity);
        }

        /// <summary>
        /// Sets the character velocity to zero through the <see cref="Rigidbody2D"/>.
        /// </summary>
        public void StandIdle() {
            this.RBody.velocity = new Vector2(0f, this.RBody.velocity.y);
        }

        /// <summary>
        /// Unfreezes the character position through the <see cref="Rigidbody2D"/>.
        /// </summary>
        public virtual void UnfreezePosition() {
            this.RBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        #endregion
    }
}
