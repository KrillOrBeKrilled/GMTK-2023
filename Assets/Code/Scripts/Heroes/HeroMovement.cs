using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Heroes {
    public class HeroMovement : MonoBehaviour
    {
        public float MovementSpeed = 4f;
        public float JumpForce = 100f;
        public float StuckTimerThreshold = 5f;
        [SerializeField] private float _positionThreshold = 0.1f;

        public AK.Wwise.Event HeroJumpEvent;

        private Rigidbody2D _rigidbody;
        private Animator _animator;

        private bool _isMoving;
        private bool _isStunned;
        private bool _maybeStuck;
        private Vector3 _prevPosition = Vector3.zero;
        private Coroutine _stunCoroutine;

        public UnityEvent<float, float, float> OnHeroIsStuck;

        // Examples
        // - 0.2 is 20% speed reduction
        // - 0.7 is 70% speed reduction
        // Clamped between [0,1]
        private float _speedPenalty = 0f;
        private static readonly int JumpKey = Animator.StringToHash("jump");
        private static readonly int XSpeedKey = Animator.StringToHash("xSpeed");
        private static readonly int YSpeedKey = Animator.StringToHash("ySpeed");

        public void ToggleMoving(bool isMoving)
        {
            this._isMoving = isMoving;
        }

        public void ResetSpeedPenalty() {
            this._speedPenalty = 0f;
        }

        public void SetSpeedPenalty(float newPenalty) {
            this._speedPenalty = newPenalty;
            this._speedPenalty = Mathf.Clamp(this._speedPenalty, 0f, 1f);
        }

        public void AddSpeedPenalty(float amount) {
            this._speedPenalty += amount;
            this._speedPenalty = Mathf.Clamp(this._speedPenalty, 0f, 1f);
        }

        public void Jump(float jumpForce = 0f)
        {
            // Add a little bit more jump force from the applied speed penalty to better prevent getting stuck
            if (jumpForce > 0f) _rigidbody.AddForce(Vector2.up * jumpForce);
            else _rigidbody.AddForce(Vector2.up * (JumpForce + JumpForce * _speedPenalty / 2f));
        
            _animator.SetTrigger(JumpKey);
        
            HeroJumpEvent.Post(gameObject);
        }

        public void Stun(float duration) {
            if (this._stunCoroutine != null) {
                this.StopCoroutine(this._stunCoroutine);
            }

            this._stunCoroutine = this.StartCoroutine(this.StunCoroutine(duration));
        }

        public void ThrowHeroBack(float stunDuration, float throwForce) {
            this.Stun(stunDuration);
            Vector2 explosionVector = new Vector2(-1f, 0.7f) * throwForce;
            this._rigidbody.AddForce(explosionVector, ForceMode2D.Impulse);
        }

        private void Awake()
        {
            this.TryGetComponent(out this._rigidbody);
            this.TryGetComponent(out this._animator);
            this.OnHeroIsStuck = new UnityEvent<float, float, float>();
            this._isStunned = false;
            this._isMoving = true;
        }

        private void Update() {
            if (this._isStunned)
                return;

            float speed = this.MovementSpeed * (1f - this._speedPenalty);
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            if (!this._isMoving)
            {
                this._rigidbody.velocity = Vector2.zero;
            }

            this._animator.SetFloat(XSpeedKey, Mathf.Abs(this._rigidbody.velocity.x));
            this._animator.SetFloat(YSpeedKey, Mathf.Abs(this._rigidbody.velocity.y));
        }

        private void FixedUpdate()
        {
            if (this._isMoving && !this._maybeStuck && Vector3.Distance(this.transform.position, this._prevPosition) < this._positionThreshold)
            {
                this._maybeStuck = true;
                this.StartCoroutine(this.ConfirmHeroIsStuck());
            }

            // Keep track of previous position to check if the hero is stuck at any point in time
            this._prevPosition = this.transform.position;
        }

        private IEnumerator StunCoroutine(float duration) {
            this._isStunned = true;
            yield return new WaitForSeconds(duration);
            this._isStunned = false;
            this._stunCoroutine = null;
        }

        // A method created specifically for UGS Analytics that checks if the hero is stuck at any point in time
        private IEnumerator ConfirmHeroIsStuck()
        {
            yield return new WaitForSeconds(this.StuckTimerThreshold);

            if (!this._isMoving || Vector3.Distance(this.transform.position, this._prevPosition) > this._positionThreshold)
            {
                this._maybeStuck = false;
                yield break;
            }

            var position = this.transform.position;
            this.OnHeroIsStuck.Invoke(position.x, position.y, position.z);

            this._maybeStuck = false;
        }
    }
}
