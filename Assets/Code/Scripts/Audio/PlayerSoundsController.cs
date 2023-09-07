using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// PlayerSoundsController
//*******************************************************************************************
namespace Audio {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the player, acting as an intermediate between
    /// the AudioManager and PlayerController classes.
    /// <remarks> Exposes methods to the PlayerController that invoke the UnityEvents the
    /// AudioManager is subscribed to. </remarks>
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

        /// <summary>
        /// Plays SFX associated with changing the selected tile for deployment on the tilemap grid.
        /// </summary>
        public void OnTileSelectMove() {
            this._onTileSelectMove?.Invoke();
        }

        /// <summary>
        /// Plays SFX associated with deploying a trap on selected tile spaces.
        /// </summary>
        public void OnTileSelectConfirm() {
            this._onTileSelectConfirm?.Invoke();
        }

        /// <summary>
        /// Plays SFX associated with the building of traps.
        /// </summary>
        public void OnStartBuild() {
            this._onStartBuild?.Invoke();
        }

        /// <summary>
        /// Stops SFX associated with the building of traps.
        /// </summary>
        public void OnStopBuild() {
            this._onStopBuild?.Invoke();
        }

        /// <summary>
        /// Plays SFX associated with completing the building of traps.
        /// </summary>
        public void OnBuildComplete() {
            this._onBuildComplete?.Invoke();
        }

        /// <summary>
        /// Plays character SFX associated with death.
        /// </summary>
        public void OnHenDeath() {
            this._onHenDeath?.Invoke();
        }

        /// <summary>
        /// Plays character SFX associated with jumping. 
        /// </summary>
        public void OnHenJump() {
            this._onHenJump?.Invoke();
        }
    }
}
