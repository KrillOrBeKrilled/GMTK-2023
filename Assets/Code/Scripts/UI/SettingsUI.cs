using Audio;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class SettingsUI : MonoBehaviour {
    [SerializeField] private Toggle _skipDialogueToggle;
    [SerializeField] private Toggle _muteMusicToggle;
    [SerializeField] private Toggle _muteSfxToggle;

    private void Awake() {
      this._skipDialogueToggle.isOn = PlayerPrefsManager.ShouldSkipDialogue();
      this._skipDialogueToggle.onValueChanged.AddListener(this.OnSkipToggleValueChanged);

      this._muteMusicToggle.isOn = PlayerPrefsManager.IsMusicMuted();
      this._muteMusicToggle.onValueChanged.AddListener(this.OnMuteMusicToggleValueChanged);

      this._muteSfxToggle.isOn = PlayerPrefsManager.AreSfxMuted();
      this._muteSfxToggle.onValueChanged.AddListener(this.OnMuteSfxToggleValueChanged);
    }

    private void OnSkipToggleValueChanged(bool skipDialogue) {
      PlayerPrefsManager.SetSkipDialogue(skipDialogue);
    }

    private void OnMuteMusicToggleValueChanged(bool muteMusic) {
      PlayerPrefsManager.SetMuteMusic(muteMusic);

      if (muteMusic) {
        Jukebox.Instance.StopMusic();
      } else {
        Jukebox.Instance.PlayMusic();
      }
    }

    private void OnMuteSfxToggleValueChanged(bool muteSfx) {
      PlayerPrefsManager.SetMuteSfx(muteSfx);
    }
  }
}
