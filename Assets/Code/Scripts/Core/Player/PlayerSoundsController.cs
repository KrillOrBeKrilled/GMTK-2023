using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// PlayerSoundsController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the player, acting as an intermediary
    /// between the AudioManager and PlayerController classes.
    /// <remarks> Exposes methods to the PlayerController that invoke the UnityEvents the
    /// AudioManager is subscribed to. </remarks>
    /// </summary>
    internal class PlayerSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onTileSelectMove,
            _onTileSelectConfirm,
            _onHenDeath,
            _onHenJump;

        /// <summary>
        /// Plays SFX associated with changing the selected tile for deployment on the tilemap grid.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onTileSelectMove"/> event. </remarks>
        internal void OnTileSelectMove() {
            this._onTileSelectMove?.Invoke();
        }

        /// <summary> Plays SFX associated with deploying a trap on selected tile spaces. </summary>
        /// <remarks> Invokes the <see cref="_onTileSelectConfirm"/> event. </remarks>
        internal void OnTileSelectConfirm() {
            this._onTileSelectConfirm?.Invoke();
        }

        /// <summary> Plays character SFX associated with death. </summary>
        /// <remarks> Invokes the <see cref="_onHenDeath"/> event. </remarks>
        internal void OnHenDeath() {
            this._onHenDeath?.Invoke();
        }

        /// <summary> Plays character SFX associated with jumping. </summary>
        /// <remarks> Invokes the <see cref="_onHenJump"/> event. </remarks>
        internal void OnHenJump() {
            this._onHenJump?.Invoke();
        }
    }
}
