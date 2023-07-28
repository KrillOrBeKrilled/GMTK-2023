using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// PlayerSoundsController
//*******************************************************************************************
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
        _onTileSelectMove?.Invoke();
    }

    public void OnTileSelectConfirm() {
        _onTileSelectConfirm?.Invoke();
    }
    
    public void OnStartBuild() {
        _onStartBuild?.Invoke();
    }
    
    public void OnStopBuild() {
        _onStopBuild?.Invoke();
    }

    public void OnBuildComplete() {
        _onBuildComplete?.Invoke();
    }
    
    public void OnHenDeath() {
        _onHenDeath?.Invoke();
    }
    
    public void OnHenJump() {
        _onHenJump?.Invoke();
    }
}
