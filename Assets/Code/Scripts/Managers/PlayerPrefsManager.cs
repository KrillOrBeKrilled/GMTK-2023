using Audio;
using UnityEngine;

namespace Managers {
  public static class PlayerPrefsManager {
    private const string SkipDialogueKey = "skipDialogue";
    private const string MuteMusicKey = "muteMusic";
    private const string MuteSfxKey = "muteSfx";

    public static bool ShouldSkipDialogue() {
      if (!PlayerPrefs.HasKey(SkipDialogueKey)) {
        return false;
      }

      bool shouldSkipDialogue = PlayerPrefs.GetInt(SkipDialogueKey) == 1;
      return shouldSkipDialogue;
    }

    public static bool IsMusicMuted() {
      if (!PlayerPrefs.HasKey(MuteMusicKey)) {
        return false;
      }

      bool isMusicMuted = PlayerPrefs.GetInt(MuteMusicKey) == 1;
      return isMusicMuted;
    }

    public static bool AreSfxMuted() {
      if (!PlayerPrefs.HasKey(MuteSfxKey)) {
        return false;
      }

      bool areSfxMuted = PlayerPrefs.GetInt(MuteSfxKey) == 1;
      return areSfxMuted;
    }

    public static void SetSkipDialogue(bool value) {
      PlayerPrefs.SetInt(SkipDialogueKey, value ? 1 : 0);
    }

    public static void SetMuteMusic(bool value) {
      PlayerPrefs.SetInt(MuteMusicKey, value ? 1 : 0);
      Jukebox.Instance.SetIsMusicMuted(value);
    }

    public static void SetMuteSfx(bool value) {
      PlayerPrefs.SetInt(MuteSfxKey, value ? 1 : 0);
      AudioManager.Instance.SetAreSfxMuted(value);
    }
  }
}
