using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// DialogueSoundsController
//*******************************************************************************************
namespace KrillOrBeKrilled.Dialogue {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the dialogue events, acting as an
    /// intermediary between the AudioManager and PlayerController classes.
    /// <remarks> Exposes methods to the Hero that invoke the UnityEvents the
    /// AudioManager is subscribed to. </remarks>
    /// </summary>
    public class DialogueSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onHenSpeak,
            _onHeroSpeak,
            _onBossSpeak;
        
        /// <summary> Plays SFX associated with the hen (Hendall) speaking dialogue. </summary>
        /// <remarks> Invokes the <see cref="_onHenSpeak"/> event. </remarks>
        public void OnHenSpeak() {
            this._onHenSpeak?.Invoke();
        }
        
        /// <summary> Plays SFX associated with the hero speaking dialogue. </summary>
        /// <remarks> Invokes the <see cref="_onHeroSpeak"/> event. </remarks>
        public void OnHeroSpeak() {
            this._onHeroSpeak?.Invoke();
        }
        
        /// <summary> Plays SFX associated with the boss (Dogan) speaking dialogue. </summary>
        /// <remarks> Invokes the <see cref="_onBossSpeak"/> event. </remarks>
        public void OnBossSpeak() {
            this._onBossSpeak?.Invoke();
        }
    }
}
