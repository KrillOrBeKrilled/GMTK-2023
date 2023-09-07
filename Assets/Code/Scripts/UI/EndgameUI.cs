using TMPro;
using UnityEngine;

//*******************************************************************************************
// EndgameUI
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Manages the Game Over menu, enabling and disabling buttons to advance to new
    /// levels and replay levels and displaying level text messages.
    /// </summary>
    public class EndgameUI : MonoBehaviour {
        [Tooltip("Displays the end game text message.")]
        [SerializeField] private TMP_Text _titleText;
        [Tooltip("Button to send the player to the next level.")]
        [SerializeField] private GameObject _nextLevelButton;
        [Tooltip("Button to replay the level.")]
        [SerializeField] private GameObject _playAgainButton;

        /// <summary>
        /// Displays the Game Over menu with text and enables the button to advance to the next level.
        /// </summary>
        /// <param name="message"> The text message to be displayed on the Game Over menu. </param>
        /// <remarks> Prevents the player from replaying the level by disabling the associated button. </remarks>
        public void ShowHenWon(string message) {
            this.gameObject.SetActive(true);
            this._titleText.text = message;

            this._nextLevelButton.SetActive(true);
            this._playAgainButton.SetActive(false);
        }

        /// <summary> Displays the Game Over menu with text and enables the button to replay the level. </summary>
        /// <param name="message"> The text message to be displayed on the Game Over menu. </param>
        /// <remarks> Prevents the player from advancing to the next level by disabling the associated button. </remarks>
        public void ShowHenLost(string message) {
            this.gameObject.SetActive(true);
            this._titleText.text = message;

            this._nextLevelButton.SetActive(false);
            this._playAgainButton.SetActive(true);
        }
    }
}
