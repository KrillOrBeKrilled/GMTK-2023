using KrillOrBeKrilled.Traps;
using UnityEngine;

//*******************************************************************************************
// Pawn
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.Commands {
    /// <summary>
    /// Parent class to handle simple movement methods for controlling a character
    /// adapted to the Command pattern for easy command execution.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : MonoBehaviour {
        [SerializeField] protected float Speed;
        [SerializeField] protected float FallVelocityLimit;
        [SerializeField] protected float GlideVelocityLimit;
        [SerializeField] protected Vector2 GroundedCheckBoxSize;
        [SerializeField] protected Vector2 GroundedCheckBoxOffset;
        [SerializeField] protected LayerMask GroundedLayerMask;
        protected Rigidbody2D RBody;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            this.RBody = this.GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate() {
            if (this.RBody.velocity.y < this.FallVelocityLimit) {
                this.RBody.velocity = new Vector2(this.RBody.velocity.x, this.FallVelocityLimit);
            }
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
        /// Adds a force of <see cref="JumpForce"/> to the character along the y-axis through the
        /// <see cref="Rigidbody2D"/>.
        /// </summary>
        /// <param name="jumpForce"> The jump force to apply. </param>
        public virtual void Jump(float jumpForce) {
            this.RBody.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
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
        /// <param name="glideSpeedMultiplier"> The multiplier for the x-Axis speed. </param>
        public virtual void Glide(float moveInput, float glideSpeedMultiplier) {
            float clampedYVelocity = Mathf.Max(this.RBody.velocity.y, this.GlideVelocityLimit);
            this.RBody.velocity = new Vector2(moveInput * glideSpeedMultiplier * this.Speed, clampedYVelocity);
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
