using System.Collections;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public float MovementSpeed = 4f;
    public float JumpForce = 100f;
    
    public AK.Wwise.Event HeroJumpEvent;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private bool _isMoving;
    private bool _isStunned;
    private Coroutine _stunCoroutine;

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
        _isMoving = isMoving;
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

    public void Jump()
    {
        _rigidbody.AddForce(Vector2.up * JumpForce);
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
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _animator);
        this._isStunned = false;
        this._isMoving = true;
    }

    private void Update() {
        if (this._isStunned)
            return;
        
        float speed = this.MovementSpeed * (1f - this._speedPenalty);
        _rigidbody.velocity = new Vector2(speed, _rigidbody.velocity.y);

        if (!_isMoving)
        {
            _rigidbody.velocity = Vector2.zero;
        }
        
        _animator.SetFloat(XSpeedKey, Mathf.Abs(_rigidbody.velocity.x));
        _animator.SetFloat(YSpeedKey, Mathf.Abs(_rigidbody.velocity.y));
    }

    private IEnumerator StunCoroutine(float duration) {
        this._isStunned = true;
        yield return new WaitForSeconds(duration);
        this._isStunned = false;
        this._stunCoroutine = null;
    }
}
