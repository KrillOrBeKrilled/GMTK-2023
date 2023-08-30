using UnityEngine;

//*******************************************************************************************
// Singleton
//*******************************************************************************************
/// <summary>
/// Implements the singleton pattern to create a reference to this class in the global
/// scope upon initialization.
/// </summary>
/// <remarks> Ensures that only one instance of this class can persist at a time. </remarks>
public class Singleton<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        // Delete this object if another instance already exists.
        if (Instance != null && Instance != this as T) {
            Destroy(this.gameObject);
        } else {
            Instance = this as T;
        }
    }
}
