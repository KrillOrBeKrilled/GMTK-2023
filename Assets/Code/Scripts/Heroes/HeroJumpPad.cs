using UnityEngine;

//*******************************************************************************************
// HeroJumpPad
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes {
    /// <summary>
    /// An invisible region outlined in the Unity Editor scene viewport used to
    /// trigger hero jumps upon collision triggers with a GameObject associated with
    /// <see cref="HeroMovement"/>.
    /// </summary>
    /// <remarks> The region <see cref="BoxCollider2D"/> size and offset are automatically
    /// adjusted by <see cref="Offset"/> and <see cref="Size"/> at runtime. </remarks>
    public class HeroJumpPad : MonoBehaviour {
        [SerializeField] internal Vector2 Offset = Vector2.zero;
        [SerializeField] internal Vector2 Size = Vector2.one;
        
        [Tooltip("The force to apply to the hero's jump on contact with this jump pad.")]
        [SerializeField] internal float HeroJumpForce;

        private BoxCollider2D _collider;

        private void Awake() {
            this.TryGetComponent(out this._collider);

            this._collider.offset = this.Offset;
            this._collider.size = this.Size;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.TryGetComponent(out HeroMovement heroMovement)) {
                return;
            }

            heroMovement.Jump(HeroJumpForce);
        }

        private void OnDrawGizmos() {
            if (this._collider is null) {
                this.TryGetComponent(out this._collider);
            }

            Gizmos.color = Color.cyan;
            var cubePosition = this.transform.position + new Vector3(this.Offset.x, this.Offset.y, 0);
            Gizmos.DrawWireCube(cubePosition, this.Size);
        }
    }
}
