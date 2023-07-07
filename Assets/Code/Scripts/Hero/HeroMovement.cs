using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public float MovementSpeed = 4f;

    private Rigidbody2D _rigidbody;

    void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    void Update()
    {
        _rigidbody.velocity = new Vector2(MovementSpeed, _rigidbody.velocity.y);
    }
}
