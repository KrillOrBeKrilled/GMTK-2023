using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
