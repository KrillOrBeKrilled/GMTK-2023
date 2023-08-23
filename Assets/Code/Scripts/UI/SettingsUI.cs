using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI {
  public class SettingsUI : MonoBehaviour {
    [SerializeField] private Toggle _skipDialogueToggle;

    private void Awake() {
      this._skipDialogueToggle.isOn = PlayerPrefsManager.ShouldSkipDialogue();
      this._skipDialogueToggle.onValueChanged.AddListener(this.OnSkipToggleValueChanged);
    }

    private void OnSkipToggleValueChanged(bool value) {
      PlayerPrefsManager.SetSkipDialogue(value);
    }
  }
}
