using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// PlayerSoundsController
//*******************************************************************************************
namespace Audio {
    /// <summary>
    /// A class that holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the player, acting as an intermediate between
    /// the AudioManager and PlayerController classes. Exposes methods to the PlayerController
    /// to call that invoke the UnityEvents that the AudioManager is subscribed to.
    /// </summary>
    public class PlayerSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onTileSelectMove,
            _onTileSelectConfirm,
            _onStartBuild,
            _onStopBuild,
            _onBuildComplete,
            _onHenDeath,
            _onHenJump;

        public void OnTileSelectMove() {
            this._onTileSelectMove?.Invoke();
        }

        public void OnTileSelectConfirm() {
            this._onTileSelectConfirm?.Invoke();
        }

        public void OnStartBuild() {
            this._onStartBuild?.Invoke();
        }

        public void OnStopBuild() {
            this._onStopBuild?.Invoke();
        }

        public void OnBuildComplete() {
            this._onBuildComplete?.Invoke();
        }

        public void OnHenDeath() {
            this._onHenDeath?.Invoke();
        }

        public void OnHenJump() {
            this._onHenJump?.Invoke();
        }
    }
}
