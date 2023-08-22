using UnityEngine;

namespace Code.Scripts.Managers {
  public static class PlayerPrefsManager {
    private const string SkipDialogueKey = "skipDialogue";

    public static bool ShouldSkipDialogue() {
      if (!PlayerPrefs.HasKey(SkipDialogueKey)) {
        return false;
      }

      bool shouldSkipDialogue = PlayerPrefs.GetInt(SkipDialogueKey) == 1;
      return shouldSkipDialogue;
    }
  }
}
