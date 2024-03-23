using Cinemachine;
using UnityEngine;

namespace KrillOrBeKrilled.Core.Managers {
  public class CameraManager : MonoBehaviour {
    [SerializeField] public CinemachineConfiner2D Confiner;

    public void SetBounds(Collider2D newBounds) {
      if (this.Confiner == null || newBounds == null) {
        return;
      }

      this.Confiner.m_BoundingShape2D = newBounds;
      this.Confiner.InvalidateCache();
    }
  }
}