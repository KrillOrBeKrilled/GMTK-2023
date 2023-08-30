using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// EndgameTarget
//*******************************************************************************************
/// <summary>
/// Ends the game when a hero triggers collisions with this GameObject.
/// </summary>
public class EndgameTarget : MonoBehaviour {
    [Tooltip("Tracks when the hero reaches the level goal.")]
    public UnityEvent OnHeroReachedEndgameTarget { get; private set; }

    private void Awake() {
        this.OnHeroReachedEndgameTarget = new UnityEvent();
    }

    /// <remarks> Invokes the <see cref="OnHeroReachedEndgameTarget"/> event on triggered collision with
    /// a GameObject in the "Hero" layer. </remarks>
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Hero")) 
            return;

        this.OnHeroReachedEndgameTarget?.Invoke();
    }
}
