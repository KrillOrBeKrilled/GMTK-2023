using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class ResourceUIGroup : MonoBehaviour {
    private bool _isHidden;

    public void Initialize(bool hide) {
      this._isHidden = hide;

      if (this._isHidden) {
        this.Hide();
      }
    }

    public void Show() {
      if (this._isHidden)
        return;

      for (int i = 0; i < 2; i++) {
        this.transform.GetChild(i).gameObject.SetActive(true); 
      }
    }
    
    public void Hide() {
      for (int i = 0; i < this.transform.childCount; i++) {
        this.transform.GetChild(i).gameObject.SetActive(false); 
      }
    }
  }
}