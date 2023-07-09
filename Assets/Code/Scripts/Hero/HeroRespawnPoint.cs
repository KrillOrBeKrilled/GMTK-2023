using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRespawnPoint : MonoBehaviour
{
	public Vector2 Offset = Vector2.zero;
	public Vector2 Size = Vector2.one;
    
	private BoxCollider2D _collider;

	private void Awake()
	{
		TryGetComponent(out _collider);

		_collider.offset = Offset;
		_collider.size = Size;
	}
    
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.TryGetComponent(out Hero hero))
		{
			return;
		}

		hero.SetRespawnPoint(this);
	}

	private void OnDrawGizmos()
	{
		if (_collider is null)
		{
			TryGetComponent(out _collider);
		}
        
		Gizmos.color = Color.magenta;
		var cubePosition = transform.position + new Vector3(Offset.x, Offset.y, 0);
		Gizmos.DrawWireCube(cubePosition, Size);
	}
}