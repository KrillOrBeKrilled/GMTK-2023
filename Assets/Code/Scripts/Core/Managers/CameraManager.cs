using Cinemachine;
using KrillOrBeKrilled.Core.Cameras;
using UnityEngine;

namespace KrillOrBeKrilled.Core.Managers {
  public class CameraManager : MonoBehaviour {
    [SerializeField] private CinemachineConfiner2D Confiner;
    [SerializeField] private CameraSwitcher _cameraSwitcher;
    [SerializeField] private CameraShaker _cameraShaker;

    public void SetBounds(Collider2D newBounds) {
      if (this.Confiner == null || newBounds == null) {
        return;
      }

      this.Confiner.m_BoundingShape2D = newBounds;
      this.Confiner.InvalidateCache();
    }
    
    
  }
}