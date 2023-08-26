using UnityEngine;

namespace Heroes {
	public class HeroRespawnPoint : MonoBehaviour
	{
		public Vector2 Offset = Vector2.zero;
		public Vector2 Size = Vector2.one;

		private BoxCollider2D _collider;

		private void Awake()
		{
			this.TryGetComponent(out this._collider);

			this._collider.offset = this.Offset;
			this._collider.size = this.Size;
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
			if (this._collider is null)
			{
				this.TryGetComponent(out this._collider);
			}

			Gizmos.color = Color.magenta;
			var cubePosition = this.transform.position + new Vector3(this.Offset.x, this.Offset.y, 0);
			Gizmos.DrawWireCube(cubePosition, this.Size);
		}
	}
}
