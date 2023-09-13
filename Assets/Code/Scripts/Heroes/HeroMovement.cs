using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Managers.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// HeroMovement
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes {
    /// <summary>
    /// Encompasses all logic associated with hero movement, including speed,
    /// jump settings, responses to collisions with traps, animations and SFX.
    /// </summary>
    /// <remarks> Sends UGS Analytics data when the hero gets stuck. </remarks>
    public class HeroMovement : MonoBehaviour {
        public float MovementSpeed = 4f;

        [Tooltip("Default jump force to apply to the hero.")]
        public float JumpForce = 100f;
        
        private bool _isMoving;

        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private static readonly int JumpKey = Animator.StringToHash("jump");
        private static readonly int XSpeedKey = Animator.StringToHash("xSpeed");
        private static readonly int YSpeedKey = Animator.StringToHash("ySpeed");

        // -------------- Trap Effects ---------------
        private bool _isStunned;
        private Coroutine _stunCoroutine;
        
        // Examples
        // - 0.2 is 20% speed reduction
        // - 0.7 is 70% speed reduction
        [Tooltip("Clamped between [0,1] as a speed reduction percentage.")]
        private float _speedPenalty = 0f;
        
        // ------------- Sound Effects ---------------
        private HeroSoundsController _soundsController;
        
        // -------------- UGS Analytics --------------
        [Tooltip("Duration of time before confirming that the hero is stuck in the same position.")]
        public float StuckTimerThreshold = 5f;
        
        [Tooltip("Range for the GameObject position to fall within to be considered stuck.")]
        [SerializeField] private float _positionThreshold = 0.1f;
        
        [Tooltip("Tracks the position of this GameObject from the previous frame.")]
        private Vector3 _prevPosition = Vector3.zero;
        private bool _maybeStuck;

        [Tooltip("Tracks when the hero gets stuck.")]
        public UnityEvent<float, float, float> OnHeroIsStuck;

        private void Awake() {
            this.TryGetComponent(out this._rigidbody);
            this.TryGetComponent(out this._animator);
            this.OnHeroIsStuck = new UnityEvent<float, float, float>();
            this._isStunned = false;
            this._isMoving = true;
        }
        
        public void Initialize(HeroSoundsController soundsController) {
            this._soundsController = soundsController;
        }

        private void Update() {
            if (this._isStunned)
                return;

            float speed = this.MovementSpeed * (1f - this._speedPenalty);
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            if (!this._isMoving) {
                this._rigidbody.velocity = Vector2.zero;
            }

            this._animator.SetFloat(XSpeedKey, Mathf.Abs(this._rigidbody.velocity.x));
            this._animator.SetFloat(YSpeedKey, Mathf.Abs(this._rigidbody.velocity.y));
        }

        private void FixedUpdate() {
            if (this._isMoving && !this._maybeStuck && 
                Vector3.Distance(this.transform.position, this._prevPosition) < this._positionThreshold) {
                this._maybeStuck = true;
                this.StartCoroutine(this.ConfirmHeroIsStuck());
            }

            // Keep track of previous position to check if the hero is stuck at any point in time
            this._prevPosition = this.transform.position;
        }
        
        /// <summary> Enables or disables the hero's ability to move. </summary>
        /// <param name="isMoving"> The hero's new movement status. </param>
        public void ToggleMoving(bool isMoving) {
            this._isMoving = isMoving;
        }
        
        /// <summary>
        /// Applies a force to the hero along the positive y-axis and triggers associated jump animations
        /// and SFX.
        /// </summary>
        /// <param name="jumpForce"> The jump force to apply to the hero. If this value is not provided
        /// and is zero or below, the default <see cref="JumpForce"/> will be applied instead. </param>
        /// <remarks> Accessed when the hero collides with a <see cref="HeroJumpPad"/>. </remarks>
        public void Jump(float jumpForce = 0f) {
            // Add a little bit more jump force from the applied speed penalty to better prevent getting stuck
            if (jumpForce > 0f) 
                _rigidbody.AddForce(Vector2.up * jumpForce);
            else 
                _rigidbody.AddForce(Vector2.up * (JumpForce + JumpForce * _speedPenalty / 2f));
        
            _animator.SetTrigger(JumpKey);

            _soundsController.OnHeroJump();
        }

        //========================================
        // Trap Effects
        //========================================

        /// <summary> Sets the <see cref="_speedPenalty"/> to reduce the hero movement speed. </summary>
        /// <param name="newPenalty"> The new speed penalty to limit hero movement. </param>
        /// <remarks> The provided speed penalty is clamped between [0,1] as a percentage value. </remarks>
        public void SetSpeedPenalty(float newPenalty) {
            this._speedPenalty = newPenalty;
            this._speedPenalty = Mathf.Clamp(this._speedPenalty, 0f, 1f);
        }

        /// <summary> Increments the <see cref="_speedPenalty"/> to reduce the hero movement speed. </summary>
        /// <param name="amount"> The speed penalty value to increment the current <see cref="_speedPenalty"/> to
        /// limit hero movement. </param>
        /// <remarks> The incremented speed penalty is clamped between [0,1] as a percentage value. </remarks>
        public void AddSpeedPenalty(float amount) {
            this._speedPenalty += amount;
            this._speedPenalty = Mathf.Clamp(this._speedPenalty, 0f, 1f);
        }
        
        /// <summary> Resets the <see cref="_speedPenalty"/> to return the hero movement speed to normal. </summary>
        public void ResetSpeedPenalty() {
            this._speedPenalty = 0f;
        }

        /// <summary>
        /// Applies a force along the negative x-axis and positive y-axis to the hero and stuns the hero.
        /// </summary>
        /// <param name="stunDuration"> The duration of time to stun the hero. </param>
        /// <param name="throwForce"> Scales the knock back force applied to the hero. </param>
        public void ThrowHeroBack(float stunDuration, float throwForce) {
            this.Stun(stunDuration);
            Vector2 explosionVector = new Vector2(-1f, 0.7f) * throwForce;
            this._rigidbody.AddForce(explosionVector, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Resets the stun duration if the hero is currently stunned and stuns the hero for a new duration of time.
        /// </summary>
        /// <param name="duration"> The duration of time to stun the hero. </param>
        private void Stun(float duration) {
            if (this._stunCoroutine is not null) {
                this.StopCoroutine(this._stunCoroutine);
            }

            this._stunCoroutine = this.StartCoroutine(this.StunCoroutine(duration));
        }

        /// <summary> Freezes the hero is place for a duration of time. </summary>
        /// <param name="duration"> The duration of time to stun the hero in place. </param>
        /// <remarks> The coroutine is started by <see cref="Stun"/>. </remarks>
        private IEnumerator StunCoroutine(float duration) {
            this._isStunned = true;
            yield return new WaitForSeconds(duration);
            this._isStunned = false;
            this._stunCoroutine = null;
        }
        
        //========================================
        // UGS Analytics
        //========================================

        /// <summary>
        /// Checks that the hero has remained within the distance range defined by <see cref="_positionThreshold"/>
        /// for <see cref="StuckTimerThreshold"/> duration of time.
        /// </summary>
        /// <remarks> If the hero remains in a similar position for the duration of time, invokes the
        /// <see cref="OnHeroIsStuck"/> event to send hero stuck positional data to UGS analytics.
        /// The coroutine is started by <see cref="FixedUpdate"/>. </remarks>
        private IEnumerator ConfirmHeroIsStuck() {
            yield return new WaitForSeconds(this.StuckTimerThreshold);

            if (!this._isMoving || Vector3.Distance(this.transform.position, this._prevPosition) > this._positionThreshold) {
                this._maybeStuck = false;
                yield break;
            }

            var position = this.transform.position;
            this.OnHeroIsStuck.Invoke(position.x, position.y, position.z);

            this._maybeStuck = false;
        }
    }
}
