using Heroes;
using Player;
using UGSAnalytics;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

//*******************************************************************************************
// GameManager
//*******************************************************************************************
namespace Managers {
    /// <summary>
    /// Acts as a central control panel for the main workings of a level and the actors
    /// within, including cutscene sequences and the handling of events through major
    /// state changes of the <see cref="Hero"/> and <see cref="PlayerController"/>.
    /// </summary>
    /// <remarks> Acts as an intermediary between <see cref="UGS_Analytics"/> and the
    /// other actors of the level to collect and send data. </remarks>
    public class GameManager : Singleton<GameManager> {
        [Header("References")]
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private EndgameTarget _endgameTarget;

        [Header("Dialogue References")]
        [SerializeField] private DialogueRunner _dialogueRunner;
        [SerializeField] private CameraSwitcher _cameraSwitcher;
        [SerializeField] private CameraShaker _cameraShaker;

        [Header("Heroes")]
        [SerializeField] private Hero _hero;

        // ----------------- Events ------------------
        [Tooltip("Tracks when hero and UI data are set up and this manager sets up other critical gameplay system " +
                 "references.")]
        public UnityEvent OnSetupComplete { get; private set; }
        [Tooltip("Tracks when the main gameplay loop begins.")]
        public UnityEvent OnStartLevel { get; private set; }
        [Tooltip("Tracks when the player beats the game.")]
        public UnityEvent<string> OnHenWon { get; private set; }
        [Tooltip("Tracks when the player loses the game.")]
        public UnityEvent<string> OnHenLost { get; private set; }
        
        protected override void Awake() {
            base.Awake();
            this.OnSetupComplete = new UnityEvent();
            this.OnStartLevel = new UnityEvent();
            this.OnHenWon = new UnityEvent<string>();
            this.OnHenLost = new UnityEvent<string>();
        }
        
        /// <remarks> Invokes the <see cref="OnSetupComplete"/> event. </remarks>
        private void Start() {
            // Setup
            this._gameUI.Initialize(this, this._playerManager);

            this._hero.ResetHero();

            this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
            this._playerManager.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
            this._playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
            this._playerManager.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);
            this._hero.OnGameOver.AddListener(this.GameWon);
            this._hero.OnHeroDied.AddListener(this.OnHeroDied);
            this._hero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

            this.OnSetupComplete?.Invoke();

            if (PlayerPrefsManager.ShouldSkipDialogue()) {
                this.SkipDialogue();
            } else {
                this._cameraSwitcher.ShowStart();
                this._cameraShaker.StopShake();
                this._dialogueRunner.StartDialogue(this._dialogueRunner.startNode);
            }

            PauseManager.Instance.SetIsPausable(true);
        }
        
        //========================================
        // Scene Management
        //========================================
        
        /// <summary> Disables pause controls and loads the MainMenu scene. </summary>
        public void LoadMainMenu() {
            PauseManager.Instance.UnpauseGame();
            PauseManager.Instance.SetIsPausable(false);
            this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadMainMenuScene);
        }

        /// <summary> Unpauses the game. </summary>
        public void LoadNextLevel() {
            PauseManager.Instance.UnpauseGame();
        }

        /// <summary> Disables pause controls and reloads the current level. </summary>
        public void ReloadThisLevel() {
            PauseManager.Instance.UnpauseGame();
            PauseManager.Instance.SetIsPausable(false);
            this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.ReloadCurrentScene);
        }
        
        //========================================
        // Level Sequence
        //========================================

        /// <summary> Triggers the sequence to make the hero enter the level. </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("enter_hero")]
        public void EnterHero() {
            this._hero.EnterLevel();
        }

        /// <summary> Enables player controls and recording features, hero movement, and timed coin earning. </summary>
        /// <remarks> Invokes the <see cref="OnStartLevel"/> event. Can be accessed as a YarnCommand.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, begins the
        /// recorded input playback feature. Otherwise, begins recording the player input. </p></remarks>
        [YarnCommand("start_level")]
        public void StartLevel() {
            this._playerManager.PlayerController.StartSession();

            this._hero.StartRunning();
            CoinManager.Instance.StartCoinEarning();
            this.OnStartLevel?.Invoke();
        }

        /// <summary>
        /// Aborts the dialogue player if the dialogue is actively running and immediately focuses the camera
        /// on the player before beginning the level gameplay.
        /// </summary>
        public void SkipDialogue() {
            if (this._dialogueRunner.IsDialogueRunning) {
                this._dialogueRunner.Stop();
            }

            this._cameraShaker.StopShake();
            this._cameraSwitcher.ShowPlayer();
            this.StartLevel();
        }

        //========================================
        // Event Listeners
        //========================================

        /// <summary>
        /// Ends the game and records analytics player death data if the player state is <see cref="GameOverState"/>. 
        /// </summary>
        /// <param name="state"> The <see cref="PlayerController"/> state. </param>
        /// <param name="xPos"> The player's current position along the x-axis. </param>
        /// <param name="yPos"> The player's current position along the y-axis. </param>
        /// <param name="zPos"> The player's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnPlayerStateChanged"/> event. </remarks>
        private void OnPlayerStateChanged(IPlayerState state, float xPos, float yPos, float zPos) {
            if (state is not GameOverState)
                return;

            this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");

            // Send Analytics data
            if (UGS_Analytics.Instance is null) 
                return;

            UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
        }
        
        /// <summary>
        /// Disables the player input and hero movement, destroying the player GameObject, and triggers the loss UI.
        /// </summary>
        /// <param name="message"> The message to be displayed on the loss UI. </param>
        /// <remarks> Helper for <see cref="GameManager.OnPlayerStateChanged"/>.
        /// Invokes the <see cref="OnHenLost"/> event.</remarks>
        private void HenDied(string message) {
            this._playerManager.PlayerController.DisablePlayerInput();
            this._hero.HeroMovement.ToggleMoving(false);

            Destroy(this._playerManager.gameObject);
            this.OnHenLost?.Invoke(message);
        }

        /// <summary> Records analytics trap switching data. </summary>
        /// <param name="trapIndex"> The most recently selected trap index. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnSelectedTrapIndexChanged"/> event. </remarks>
        private void SelectedTrapIndexChanged(int trapIndex) {
            var isAffordable = this._playerManager.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;

            // Send Analytics data
            if (UGS_Analytics.Instance is null) 
                return;

            UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
        }

        /// <summary> Records analytics trap deployment data. </summary>
        /// <param name="trapIndex"> The most recently selected trap index. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnTrapDeployed"/> event. </remarks>
        private void OnTrapDeployed(int trapIndex) {
            if (UGS_Analytics.Instance is null)
                return;

            UGS_Analytics.DeployTrapCustomEvent(trapIndex);
        }
        
        /// <summary> Records analytics hero death data. </summary>
        /// <param name="numberLives"> The number of lives remaining for the hero. </param>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
        private void OnHeroDied(int numberLives, float xPos, float yPos, float zPos) {
            // Send Analytics data
            if (UGS_Analytics.Instance is null) 
                return;

            UGS_Analytics.HeroDiedCustomEvent(numberLives, xPos, yPos, zPos);
        }

        /// <summary> Records analytics hero stuck data. </summary>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="HeroMovement.OnHeroIsStuck"/> event. </remarks>
        private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
            // Send Analytics data
            if (UGS_Analytics.Instance is null) 
                return;

            UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
        }

        /// <summary> Disables player input and triggers the win UI. </summary>
        /// <remarks> Subscribed to the <see cref="Hero.OnGameOver"/> event. Invokes the <see cref="OnHenWon"/> event.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, does nothing.
        /// Otherwise, stops recording the player input and creates a file for the recorded input. </p></remarks>
        private void GameWon() {
            this._playerManager.PlayerController.DisablePlayerInput();
            this.OnHenWon?.Invoke("The Hero was stopped, good work Hendall!");
        }

        /// <summary> Disables the player input and hero movement, and triggers the loss UI. </summary>
        /// <remarks> Subscribed to the <see cref="EndgameTarget.OnHeroReachedEndgameTarget"/> event.
        /// Invokes the <see cref="OnHenLost"/> event. </remarks>
        private void HeroReachedLevelEnd() {
            this._playerManager.PlayerController.DisablePlayerInput();
            this._hero.HeroMovement.ToggleMoving(false);
            this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
        }
    }
}
