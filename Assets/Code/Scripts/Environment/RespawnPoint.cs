using UnityEngine;
using UnityEngine.Serialization;

//*******************************************************************************************
// RespawnPoint
//*******************************************************************************************
namespace KrillOrBeKrilled.Environment {
	/// <summary>
	/// An invisible region outlined in the Unity Editor scene viewport used as a
	/// marker to spawn heroes in waves.
	/// </summary>
	public class RespawnPoint : MonoBehaviour {
    	[FormerlySerializedAs("Offset")] [SerializeField] private Vector2 _offset = Vector2.zero;
    	[FormerlySerializedAs("Size")] [SerializeField] private Vector2 _size = Vector2.one;
    
    	private BoxCollider2D _collider;
    
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this.TryGetComponent(out this._collider);
    
            this._collider.offset = this._offset;
            this._collider.size = this._size;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
	        // if (!other.TryGetComponent(out Hero hero)) {
	        // 	return;
	        // }
    
	        // TODO : Update to set checkpoints for heroes on the GameManager
	        // hero.SetRespawnPoint(this);
        }
    
        private void OnDrawGizmos() {
	        if (this._collider is null) {
		        this.TryGetComponent(out this._collider);
	        }
    
	        Gizmos.color = Color.magenta;
	        var cubePosition = this.transform.position + new Vector3(this._offset.x, this._offset.y, 0);
	        Gizmos.DrawWireCube(cubePosition, this._size);
        }
        
        #endregion
	}
}

