using DG.Tweening;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// GameUI
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Acts as a central control panel for initializing, activating/deactivating, and
    /// updating UI elements for various gameplay systems.
    /// </summary>
    /// <remarks> Certain elements such as UI initialization and screen fade-in effects
    /// are exposed for external use. </remarks>
    public class GameUI : MonoBehaviour {
        [Header("Game UI References")]
        [Tooltip("Used to fade the scene in and out.")]
        [SerializeField] private Image _foregroundImage;
        [Tooltip("Pause menu UI.")]
        [SerializeField] private GameObject _pauseUI;
        [Tooltip("Game Over menu UI.")]
        [SerializeField] private EndgameUI _endgameUI;
        [Tooltip("The coin counter UI text displayed during gameplay.")]
        [SerializeField] private TMP_Text _coinsText;
        [Tooltip("The trap selector toolbar displayed during gameplay.")]
        [SerializeField] private TrapSelectionBar _trapSelectionBar;
        [Tooltip("The Skip Dialogue HUD displayed during dialogue sequences.")]
        [SerializeField] private SkipDialogueUI _skipDialogueUI;

        [Header("Pause UI Events")]
        [Tooltip("Tracks when the game is paused.")]
        [SerializeField] private UnityEvent _onPaused;
        [Tooltip("Tracks when the game is unpaused.")]
        [SerializeField] private UnityEvent _onUnpaused;

        private const float FadeDuration = 0.5f;

        /// <summary>
        /// Sets up all references and listeners to operate the game UI, also invoking the initialization methods
        /// through the <see cref="TrapSelectionBar"/> and <see cref="SkipDialogueUI"/>.
        /// </summary>
        /// <param name="gameManager"> Provides events related to the game state to subscribe to. </param>
        /// <param name="playerManager"> Provides events related to the trap system to subscribe to. </param>
        public void Initialize(GameManager gameManager, PlayerManager playerManager) {
            gameManager.OnSetupComplete.AddListener(this.OnGameSetupComplete);
            gameManager.OnHenWon.AddListener(this.OnHenWon);
            gameManager.OnHenLost.AddListener(this.OnHenLost);

            this._trapSelectionBar.Initialize(playerManager);
            this._skipDialogueUI.Initialize(gameManager.OnStartLevel, gameManager.SkipDialogue);
        }

        /// <summary> Fades in the screen and invokes a function upon completion. </summary>
        /// <param name="onComplete"> The function to invoke once the fade-in effect has been completed. </param>
        public void FadeInSceneCover(UnityAction onComplete) {
            this._foregroundImage.gameObject.SetActive(true);
            this._foregroundImage
                .DOFade(1, FadeDuration)
                .OnComplete(() => onComplete?.Invoke());
        }
        
        private void Awake() {
            this._foregroundImage.gameObject.SetActive(true);
        }

        private void Start() {
            CoinManager.Instance.OnCoinAmountChanged.AddListener(this.OnCoinsUpdated);
            PauseManager.Instance.OnPauseToggled.AddListener(this.OnPauseToggled);
        }

        /// <summary> Updates the coin counter UI text. </summary>
        /// <param name="amount"> The new coin count to display on the coin counter UI. </param>
        private void OnCoinsUpdated(int amount) {
            this._coinsText.SetText($"{amount}");
        }

        /// <summary> Fades out the screen and disables the fade image upon completion. </summary>
        /// <remarks> Listens on the <see cref="GameManager.OnSetupComplete"/> event. </remarks>
        private void OnGameSetupComplete() {
            this._foregroundImage
                .DOFade(0, FadeDuration)
                .OnComplete(() => {
                this._foregroundImage.gameObject.SetActive(false);
                });
        }

        /// <summary> Enables or disables the Pause menu UI. </summary>
        /// <param name="isPaused"> Whether the game is currently paused or not. </param>
        /// <remarks> Listens on the <see cref="PauseManager.OnPauseToggled"/> event. Invokes the
        /// <see cref="_onPaused"/> and <see cref="_onUnpaused"/> events. </remarks>
        private void OnPauseToggled(bool isPaused) {
            this._pauseUI.SetActive(isPaused);

            if (isPaused) {
                this._onPaused?.Invoke();
            } else {
                this._onUnpaused?.Invoke();
            }
        }

        /// <summary> Opens the Game Over menu with a custom text message. </summary>
        /// <param name="message"> The message to display when the player has beat the level. </param>
        private void OnHenWon(string message) {
            this._endgameUI.ShowHenWon(message);
        }

        /// <summary> Opens the Game Over menu with a custom text message. </summary>
        /// <param name="message"> The message to display when the player has lost the level. </param>
        private void OnHenLost(string message) {
            this._endgameUI.ShowHenLost(message);
        }
    }
}
