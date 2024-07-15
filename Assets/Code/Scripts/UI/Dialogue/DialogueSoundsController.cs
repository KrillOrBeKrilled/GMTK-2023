using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// DialogueSoundsController
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the dialogue events, acting as an
    /// intermediary between the AudioManager and <see cref="StoryUI"/>
    /// classes.
    /// </summary>
    /// <remarks>
    /// Exposes methods to the <see cref="StoryUI"/> that invoke
    /// the UnityEvents the AudioManager is subscribed to.
    /// </remarks>
    internal class DialogueSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onHenSpeak,
            _onHeroSpeak,
            _onBossSpeak;
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Plays SFX associated with the boss (Dogan) speaking dialogue.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onBossSpeak"/> event. </remarks>
        internal void OnBossSpeak() {
            this._onBossSpeak?.Invoke();
        }
        
        /// <summary>
        /// Plays SFX associated with the hen (Hendall) speaking dialogue.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onHenSpeak"/> event. </remarks>
        internal void OnHenSpeak() {
            this._onHenSpeak?.Invoke();
        }
        
        /// <summary>
        /// Plays SFX associated with the hero speaking dialogue.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onHeroSpeak"/> event. </remarks>
        internal void OnHeroSpeak() {
            this._onHeroSpeak?.Invoke();
        }

        #endregion
    }
}
