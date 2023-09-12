using System.Collections.Generic;
using DG.Tweening;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Traps;
using KrillOrBeKrilled.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// GameUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
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
        [SerializeField] private Transform _healthBarsContainer;
        [SerializeField] private MapUI _mapUI;

        [Header("Pause UI Events")]
        [Tooltip("Tracks when the game is paused.")]
        [SerializeField] private UnityEvent _onPaused;
        [Tooltip("Tracks when the game is unpaused.")]
        [SerializeField] private UnityEvent _onUnpaused;

        [Header("Prefabs")]
        [SerializeField] private HealthBarUI _healthBarUIPrefab;

        private const float FadeDuration = 0.5f;

        /// <summary>
        /// Sets up all references and listeners to operate the game UI, also invoking the initialization methods
        /// through the <see cref="TrapSelectionBar"/> and <see cref="SkipDialogueUI"/>.
        /// </summary>
        /// <param name="gameManager"> Provides events related to the game state to subscribe to. </param>
        /// <param name="playerManager"> Provides events related to the trap system to subscribe to. </param>
        public void Initialize(UnityEvent setupComplete, UnityEvent<string> henWon, UnityEvent<string> henLost, 
            UnityEvent<Hero> heroSpawned,
            UnityEvent<int> trapIndexChanged, List<Trap> traps,
            UnityEvent onStartLevel, UnityAction onSkipDialogue,
            Transform playerTransform, Transform levelStartTransform, Transform levelEndTransform) {
            setupComplete.AddListener(this.OnGameSetupComplete);
            henWon.AddListener(this.OnHenWon);
            henLost.AddListener(this.OnHenLost);
            heroSpawned.AddListener(this.OnHeroSpawned);

            this._trapSelectionBar.Initialize(trapIndexChanged, traps);
            this._skipDialogueUI.Initialize(onStartLevel, onSkipDialogue);
            this._mapUI.Initialize(playerTransform, levelStartTransform.position.x, levelEndTransform.position.x);
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
        /// <remarks> Subscribed to the <see cref="CoinManager.OnCoinAmountChanged"/> event. </remarks>
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
        /// <remarks> Subscribed to the <see cref="PauseManager.OnPauseToggled"/> event. Invokes the
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

        private void OnHeroSpawned(Hero hero) {
            this.SetupHealthBar(hero);
            this._mapUI.RegisterHero(hero);
        }

        private void SetupHealthBar(Hero hero) {
            HealthBarUI newBar = Instantiate(this._healthBarUIPrefab, this._healthBarsContainer);
            newBar.Initialize(hero, (RectTransform)this.transform);
        }
    }
}
