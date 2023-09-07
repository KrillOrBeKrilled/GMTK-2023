using UnityEngine;

//*******************************************************************************************
// PlayerPrefsManager
//*******************************************************************************************
namespace Managers {
    /// <summary>
    /// Manages the direct manipulation of the data stored in <see cref="PlayerPrefs"/>
    /// and relays the data to external classes through exposed methods.
    /// </summary>
    public static class PlayerPrefsManager {
        // PlayerPrefs key for accessing the setting to skip the level dialogue.
        private const string SkipDialogueKey = "skipDialogue";

        /// <summary>
        /// Checks that the skip dialogue <see cref="PlayerPrefs"/> setting is toggled if the key is found
        /// within <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <returns> If the skip dialogue value in <see cref="PlayerPrefs"/> is equal to 1. If the skip dialogue
        /// key cannot be found in <see cref="PlayerPrefs"/>, returns false. </returns>
        public static bool ShouldSkipDialogue() {
            if (!PlayerPrefs.HasKey(SkipDialogueKey)) {
                return false;
            }

            bool shouldSkipDialogue = PlayerPrefs.GetInt(SkipDialogueKey) == 1;
            return shouldSkipDialogue;
        }

        /// <summary>
        /// Sets the value of the skip dialogue PlayerPref.
        /// </summary>
        /// <param name="value"> The value to set the skip dialogue PlayerPref, where true is 1 and false is 0. </param>
        public static void SetSkipDialogue(bool value) {
            PlayerPrefs.SetInt(SkipDialogueKey, value ? 1 : 0);
        }
    }
}
