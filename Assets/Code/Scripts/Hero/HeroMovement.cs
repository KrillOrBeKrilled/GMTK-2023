using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public float MovementSpeed = 4f;
    public float JumpForce = 100f;

    private Rigidbody2D _rigidbody;
    private HeroMovement _heroMovement;

    // Examples
    // - 0.2 is 20% speed reduction
    // - 0.7 is 70% speed reduction
    // Clamped between [0,1]
    private float _speedPenalty = 0f;

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    private void Update() {
        float speed = this.MovementSpeed * (1f - this._speedPenalty);
        _rigidbody.velocity = new Vector2(speed, _rigidbody.velocity.y);
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
    }
}
