using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// HeroSoundsController
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes {
    /// <summary>
    /// Holds all UnityEvents used to communicate with the AudioManager to fire
    /// off Wwise sound events associated with the hero, acting as an intermediary
    /// between the AudioManager and PlayerController classes.
    /// <remarks> Exposes methods to the Hero that invoke the UnityEvents the
    /// AudioManager is subscribed to. </remarks>
    /// </summary>
    public class HeroSoundsController : MonoBehaviour {
        [SerializeField] private UnityEvent
            _onTakeDamage,
            _onHeroJump,
            _onHeroDeath;
        
        /// <summary> Plays SFX associated with the hero taking damage. </summary>
        /// <remarks> Invokes the <see cref="_onTakeDamage"/> event. </remarks>
        internal void OnTakeDamage() {
            this._onTakeDamage?.Invoke();
        }
        
        /// <summary> Plays SFX associated with the hero jumping. </summary>
        /// <remarks> Invokes the <see cref="_onHeroJump"/> event. </remarks>
        internal void OnHeroJump() {
            this._onHeroJump?.Invoke();
        }
        
        /// <summary> Plays SFX associated with the hero dying. </summary>
        /// <remarks> Invokes the <see cref="_onHeroDeath"/> event. </remarks>
        internal void OnHeroDeath() {
            this._onHeroDeath?.Invoke();
        }
    }
}
