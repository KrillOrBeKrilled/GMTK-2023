using KrillOrBeKrilled.Core;
using KrillOrBeKrilled.Managers;
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
        [SerializeField] private GameManager _gameManager;
        [Tooltip("Used to cover the scene until the next level loads.")]
        [SerializeField] private Image _loadingScreen;
        [Tooltip("Used to screen wipe the scene in and out.")]
        [SerializeField] private ScreenWipeUI _screenWipe;
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
        [SerializeField] private ControlsUI _controlsUI;
        [SerializeField] private TrapRequirementsUI _trapRequirementsUI;

        [Header("Prefabs")]
        [SerializeField] private HealthBarUI _healthBarUIPrefab;

        private Animator _animationController;
        
        private readonly int _wipeInKey = Animator.StringToHash("screenWipeIn");
        private readonly int _wipeOutKey = Animator.StringToHash("screenWipeOut");

        private UnityAction _onComplete;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            _animationController = GetComponent<Animator>();
        }

        private void Start() {
            this._gameManager.OnSetupComplete.AddListener(this.OnGameSetupComplete);
            this._gameManager.OnHenWon.AddListener(this.OnHenWon);
            this._gameManager.OnHenLost.AddListener(this.OnHenLost);
            this._gameManager.OnHeroSpawned.AddListener(this.OnHeroSpawned);
            this._gameManager.OnSceneWillChange.AddListener(this.FadeInSceneCover);

            this._trapRequirementsUI.Initialize(this._gameManager.PlayerController.OnSelectedTrapChanged);
            this._trapSelectionBar.Initialize(
                this._gameManager.PlayerController.OnSelectedTrapChanged,
                this._gameManager.TrapController.Traps,
                this._gameManager.PlayerController.SetTrap
            );

            this._skipDialogueUI.Initialize(this._gameManager.OnStartLevel, this._gameManager.SkipDialogue);
            this._mapUI.Initialize(
                this._gameManager.PlayerManager.transform,
                this._gameManager.LevelStart.position.x,
                this._gameManager.LevelEnd.position.x
            );

            this._controlsUI.Initialize(this._gameManager.PlayerController);

            EventManager.Instance.CoinAmountChangedEvent.AddListener(this.OnCoinsUpdated);
            EventManager.Instance.PauseToggledEvent.AddListener(this.OnPauseToggled);
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        public void DisableScreenWipe() {
            this._screenWipe.gameObject.SetActive(false);
        }

        public void DisableLoadingScreen() {
            this._loadingScreen.gameObject.SetActive(false);
        }

        public void CompleteSceneChange() {
            this._loadingScreen.gameObject.SetActive(true);
            _onComplete?.Invoke();
        }

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Fades in the screen and invokes a function upon completion.
        /// </summary>
        /// <param name="onComplete"> The function to invoke once the fade-in effect has been completed. </param>
        private void FadeInSceneCover(UnityAction onComplete) {
            _onComplete = onComplete;
            
            this._screenWipe.gameObject.SetActive(true);
            this._screenWipe.SetRandomWipeShape();
            this._animationController.SetTrigger(_wipeOutKey);
        }

        /// <summary>
        /// Updates the coin counter UI text.
        /// </summary>
        /// <param name="amount"> The new coin count to display on the coin counter UI. </param>
        /// <remarks> Subscribed to the <see cref="CoinAmountChangedEvent"/> event. </remarks>
        private void OnCoinsUpdated(int amount) {
            this._coinsText.SetText($"{amount}");
        }

        /// <summary>
        /// Fades out the screen and disables the fade image upon completion.
        /// </summary>
        /// <remarks> Listens on the <see cref="GameManager.OnSetupComplete"/> event. </remarks>
        private void OnGameSetupComplete() {
            this._screenWipe.SetRandomWipeShape();
            this._animationController.SetTrigger(_wipeInKey);
        }

        /// <summary>
        /// Opens the Game Over menu with a custom text message.
        /// </summary>
        /// <param name="message"> The message to display when the player has lost the level. </param>
        private void OnHenLost(string message) {
            this._endgameUI.ShowHenLost(message);
        }

        /// <summary>
        /// Opens the Game Over menu with a custom text message.
        /// </summary>
        /// <param name="message"> The message to display when the player has beat the level. </param>
        private void OnHenWon(string message) {
            this._endgameUI.ShowHenWon(message);
        }

        /// <summary>
        /// Creates a health bar for the associated <see cref="Hero"/> and registers the hero to be represented
        /// on the <see cref="MapUI"/>.
        /// </summary>
        /// <param name="hero"> The newly spawned <see cref="Hero"/>. </param>
        private void OnHeroSpawned(Hero hero) {
            this.SetupHealthBar(hero);
            this._mapUI.RegisterHero(hero);
        }

        /// <summary>
        /// Enables or disables the Pause menu UI.
        /// </summary>
        /// <param name="isPaused"> Whether the game is currently paused or not. </param>
        /// <remarks> Subscribed to the <see cref="PauseToggledEvent"/> event. </remarks>
        private void OnPauseToggled(bool isPaused) {
            if (isPaused) {
                // TODO: Play short modal show animation
                this._pauseUI.SetActive(true);
            }
        }

        /// <summary>
        /// Instantiates a new health bar and links it to the assigned <see cref="Hero"/> <see cref="Transform"/>.
        /// </summary>
        /// <param name="hero"> The hero to receive the newly instantiated health bar. </param>
        private void SetupHealthBar(Hero hero) {
            HealthBarUI newBar = Instantiate(this._healthBarUIPrefab, this._healthBarsContainer);
            newBar.Initialize(hero, (RectTransform)this.transform);
        }

        #endregion
    }
}
