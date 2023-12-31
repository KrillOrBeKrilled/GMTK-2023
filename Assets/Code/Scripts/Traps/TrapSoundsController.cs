using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// TrapSoundsController
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with traps, acting as an intermediary
    /// between the AudioManager and <see cref="Trap"/> classes.
    /// <remarks> Exposes methods to <see cref="Trap"/> that invoke the UnityEvents
    /// the AudioManager is subscribed to. </remarks>
    /// </summary>
    public class TrapSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onStartBuild,
            _onStopBuild,
            _onBuildComplete;
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        // TODO: Update this!
        /// <summary>
        /// Plays or stops SFX associated with the building of traps.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onStartBuild"/> or <see cref="_onStopBuild"/> event. </remarks>
        internal void OnBuild(bool status) {
            if (status) {
                this._onStartBuild?.Invoke();
            } else {
                this._onStopBuild?.Invoke();
            }
        }

        /// <summary>
        /// Plays SFX associated with completing the building of traps.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onBuildComplete"/> event. </remarks>
        internal void OnBuildComplete() {
            this._onBuildComplete?.Invoke();
        }
        
        #endregion
    }
}
