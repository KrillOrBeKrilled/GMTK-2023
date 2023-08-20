using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroJumpPad : MonoBehaviour
{
    public Vector2 Offset = Vector2.zero;
    public Vector2 Size = Vector2.one;

    public float HeroJumpForce;
    
    private BoxCollider2D _collider;

    private void Awake()
    {
        TryGetComponent(out _collider);

        _collider.offset = Offset;
        _collider.size = Size;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out HeroMovement heroMovement))
        {
            return;
        }

        heroMovement.Jump(HeroJumpForce);
    }

    private void OnDrawGizmos()
    {
        if (_collider is null)
        {
            TryGetComponent(out _collider);
        }
        
        Gizmos.color = Color.cyan;
        var cubePosition = transform.position + new Vector3(Offset.x, Offset.y, 0);
        Gizmos.DrawWireCube(cubePosition, Size);
    }
}
