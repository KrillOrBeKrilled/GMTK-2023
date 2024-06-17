using System.Collections;
using System.Linq;
using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Core.Cameras;
using KrillOrBeKrilled.Core.Input;
using KrillOrBeKrilled.Core.UGSAnalytics;
using KrillOrBeKrilled.Environment;
using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Player;
using KrillOrBeKrilled.Player.PlayerStates;
using KrillOrBeKrilled.Traps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Yarn.Unity;

//*******************************************************************************************
// GameManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Acts as a central control panel for the main workings of a level and the actors
    /// within, including cutscene sequences and the handling of events through major
    /// state changes of the <see cref="Hero"/> and <see cref="PlayerCharacter"/>.
    /// </summary>
    /// <remarks> Acts as an intermediary between <see cref="UGS_Analytics"/> and the
    /// other actors of the level to collect and send data. </remarks>
    public class GameManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PauseManager _pauseManager;
        [SerializeField] private CameraManager _cameraManager;
        [field:SerializeField] public WaveManager WaveManager { get; private set; }

        [Header("Dialogue References")]
        [SerializeField] private DialogueRunner _dialogueRunner;
        
        [Header("Level")]
        [SerializeField] private EndgameTarget _endgameTargetPrefab;
        [SerializeField] private Transform _tilemapGrid;

        [Header("Testing")]
        [Tooltip("Assign if you want to test specific level. Important: Leave as NULL when done testing.")]
        [SerializeField] private LevelData _testingLevelData;
        [SerializeField] private CameraSwitcher _cameraSwitcher;
        [SerializeField] private CameraShaker _cameraShaker;


        public PlayerController PlayerController => this._playerController;
        public PlayerCharacter Player => this._playerController.Player;
        public TrapController TrapController => this._playerController.TrapController;

        public Vector3 LevelStart { get; private set; }
    
        public Transform LevelEnd => this._endgameTarget.transform;

        /// The instantiated endgameTargetPrefab.
        private EndgameTarget _endgameTarget;
        /// The instantiated defaultHeroPrefab used for story mode dialogue sequences.
        private Hero _heroActor;

        /// Contains all data on the waves and hero settings to spawn per wave that constitutes a playable level.
        private LevelData _levelData;

        // ------------- Sound Effects ---------------
        private HeroSoundsController _heroSoundsController;

        // ----------------- Events ------------------
        [Header("Events")]
        [Tooltip("Tracks when hero and UI data are set up and this manager sets up other critical gameplay system " +
                 "references.")]
        [SerializeField] private GameEvent _onSetupComplete;
        
        [Tooltip("Tracks when the main gameplay loop begins.")]
        [SerializeField] private GameEvent _onStartLevel;
        
        [Tooltip("Tracks when the player beats the game.")]
        public UnityEvent<string> OnHenWon { get; private set; }
        [Tooltip("Tracks when the player loses the game.")]
        public UnityEvent<string> OnHenLost { get; private set; }
        
        [Tooltip("Tracks when a new scene should be loaded.")]
        public UnityEvent<UnityAction> OnSceneWillChange { get; private set; }

        private bool _isGameOver;
        private Tilemap _levelTilemap;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            TryGetComponent(out this._heroSoundsController);

            this.OnHenWon = new UnityEvent<string>();
            this.OnHenLost = new UnityEvent<string>();
            this.OnSceneWillChange = new UnityEvent<UnityAction>();
        }

        /// <remarks> Invokes the <see cref="_onSetupComplete"/> event. </remarks>
        private IEnumerator Start() {
            this.CopyLevelData();
            this.SetupLevelMap();
            this.InitializeHelpers();

            // Wait for a frame so that all other scripts complete Start() method.
            yield return null;
            
            this._onSetupComplete.Raise();

            if (this._levelData.Type == LevelData.LevelType.Story) {
                this.StartStoryLevel();
            } else {
                this.StartEndlessLevel();
            }

            this._pauseManager.SetIsPausable(true);
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        #region Level Sequence
        

        /// <summary>
        /// Enables player controls and recording features, hero movement, and timed coin earning. To be used to start
        /// the level when a hero actor is already spawned during dialogue.
        /// </summary>
        /// <remarks>
        /// Invokes the <see cref="_onStartLevel"/> event. Can be accessed as a YarnCommand.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, begins the
        /// recorded input playback feature. Otherwise, begins recording the player input. </p>
        /// </remarks>
        [YarnCommand("start_level_disabled_spawn")]
        public void StartLevelNoSpawn() {
            this._playerController.StartSession();

            if (this._heroActor != null) {
                this._heroActor.StartRunning();
            }

            CoinManager.Instance.StartCoinEarning();
            ResourceSpawner.Instance.StartSpawner();
            this._onStartLevel.Raise();
        }

        /// <summary>
        /// Enables player controls and recording features, hero movement. To be used to start
        /// the level when no hero actors have been spawned in the dialogue.
        /// </summary>
        /// <remarks> Invokes the <see cref="_onStartLevel"/> event. Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("start_level_enabled_spawn")]
        public void StartLevelWithSpawn() {
            this.StartLevelNoSpawn();
            this.WaveManager.StartWaveSpawning();
        }

        /// <summary>
        /// Aborts the dialogue playback if the dialogue is actively running and starts the level.
        /// </summary>
        /// <remarks> Invokes the <see cref="EventManager.HideDialogueUIEvent"/> event. </remarks>
        public void SkipDialogue() {
            EventManager.Instance.HideDialogueUIEvent?.Invoke();
            if (this._dialogueRunner.IsDialogueRunning) {
                this._dialogueRunner.Stop();
            }

            this._cameraShaker.StopShake();
            this._cameraSwitcher.ShowPlayer();
            this.StartLevelWithSpawn();
        }

        #endregion

        #region Scene Management

        /// <summary>
        /// Disables pause controls and loads the MainMenu scene.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnSceneWillChange"/> event. </remarks>
        public void LoadMainMenu() {
            this._pauseManager.UnpauseGame();
            this._pauseManager.SetIsPausable(false);
            this.WaveManager.FreezeAllHeroes();
            this.OnSceneWillChange?.Invoke(SceneNavigationManager.LoadLobbyScene);
        }

        /// <summary>
        /// Unpauses the game.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnSceneWillChange"/> event. </remarks>
        public void LoadNextLevel() {
            this._pauseManager.UnpauseGame();
            this._pauseManager.SetIsPausable(false);
            this.WaveManager.FreezeAllHeroes();

            UnityAction onSceneLoaded;
            if (string.IsNullOrEmpty(this._levelData.NextLevelName)) {
                onSceneLoaded = SceneNavigationManager.LoadLobbyScene;
            } else {
                onSceneLoaded = () => LevelManager.Instance.LoadLevel(this._levelData.NextLevelName);;
            }

            this.OnSceneWillChange?.Invoke(onSceneLoaded);
        }

        /// <summary>
        /// Disables pause controls and reloads the current level.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnSceneWillChange"/> event. </remarks>
        public void ReloadThisLevel() {
            this._pauseManager.UnpauseGame();
            this._pauseManager.SetIsPausable(false);
            this.WaveManager.FreezeAllHeroes();
            this.OnSceneWillChange?.Invoke(SceneNavigationManager.ReloadCurrentScene);
        }

        #endregion
        
        /// <summary>
        /// Triggers the sequence to make the hero enter the level.
        /// </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("enter_hero_actor")]
        public void EnterHero() {
            this._heroActor.EnterLevel();
        }

        /// <summary>
        /// Spawns a new hero from the level data at the corresponding spawn point.
        /// </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("spawn_hero_actor")]
        public void SpawnHeroActor() {
            // TODO: MOVE TO DIALOGUE MANAGER
            this._heroActor = this.WaveManager.SpawnHero(HeroData.DefaultHero, true);
        }
        
        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods
        
        /// <summary>
        /// Create a copy of the level data for use during the round.
        /// </summary>
        private void CopyLevelData() {
            LevelData sourceData = this._testingLevelData != null ? this._testingLevelData : LevelManager.Instance.GetActiveLevelData();
            this._levelData = ScriptableObject.CreateInstance<LevelData>();
            this._levelData.Index = sourceData.Index;
            this._levelData.DialogueName = sourceData.DialogueName;
            this._levelData.NextLevelName = sourceData.NextLevelName;
            this._levelData.Type = sourceData.Type;
            this._levelData.RespawnPositions = sourceData.RespawnPositions.ToList();
            this._levelData.EndgameTargetPosition = sourceData.EndgameTargetPosition;
            this._levelData.WallsTilemapPrefab = sourceData.WallsTilemapPrefab;
            this._levelData.WavesData = new WavesData() { WavesList = sourceData.WavesData.WavesList.ToList() };
        }
        
        private void InitializeHelpers() {
            this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
            this._playerController.Player.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
            this._playerController.Player.OnSelectedTrapChanged.AddListener(this.SelectedTrapIndexChanged);
            this._playerController.Player.OnTrapDeployed.AddListener(this.OnTrapDeployed);
            this.WaveManager.Initialize(this._levelData, this._heroSoundsController, this._levelTilemap);
            
            this._playerController.Initialize(this, this._levelTilemap);
            ResourceManager.Instance.Initialize(this.PlayerController.TrapController.OnConsumeResources);
            ResourceSpawner.Instance.Initialize(this.PlayerController.transform);
            TilemapManager.Instance.Initialize(this._levelTilemap, this.PlayerController.TrapController, this._playerController.Player);
        }

        private void SetupLevelMap() {
            this._endgameTarget = Instantiate(this._endgameTargetPrefab, this._levelData.EndgameTargetPosition, 
                                              Quaternion.identity, this.transform);

            this._levelTilemap = Instantiate(this._levelData.WallsTilemapPrefab, this._tilemapGrid);
            this._cameraManager.SetBounds(this._levelData.WallsTilemapPrefab.transform.GetComponentExactlyInChildren<Collider2D>());
            this.LevelStart = this._levelData.RespawnPositions.First();
        }

        #region Level Sequence

        /// <summary>
        /// Skips the dialogue for the level.
        /// </summary>
        /// <remarks> The endless level settings will skip story dialogue by default. </remarks>
        private void StartEndlessLevel() {
            this.SkipDialogue();
        }

        /// <summary>
        /// Skips the dialogue associated with the level story event if the <see cref="PlayerPrefsManager"/>
        /// settings are enabled. Otherwise, begins playing the level dialogue.
        /// </summary>
        private void StartStoryLevel() {
            if (PlayerPrefsManager.ShouldSkipDialogue()) {
                this.SkipDialogue();
                return;
            }

            if (!this._dialogueRunner.yarnProject.NodeNames.Contains(this._levelData.DialogueName)) {
                Debug.LogError("Missing or Incorrect Dialogue Name, make sure provided dialogue name value is correct");
                return;
            }

            EventManager.Instance.ShowDialogueUIEvent?.Invoke();
            this._dialogueRunner.StartDialogue(this._levelData.DialogueName);
            this._dialogueRunner.onDialogueComplete.AddListener(() => EventManager.Instance.HideDialogueUIEvent?.Invoke());
        }

        #endregion

        #region UGSAnalytics

        /// <summary>
        /// Ends the game and records analytics player death data if the player state is <see cref="GameOverState"/>.
        /// </summary>
        /// <param name="state"> The <see cref="PlayerCharacter"/> state. </param>
        /// <param name="pos"> The player's current position. </param>
        /// <remarks> Subscribed to the <see cref="PlayerCharacter.OnPlayerStateChanged"/> event. </remarks>
        private void OnPlayerStateChanged(IPlayerState state, Vector3 pos) {
            if (state is not DeadState) {
                return;
            }

            this.HenDied("I'm thinking of reconsidering that promotion I mentioned earlier!");

            if (UGS_Analytics.Instance is null) {
                return;
            }

            UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, pos);
        }

        /// <summary>
        /// Records analytics trap deployment data.
        /// </summary>
        /// <param name="trap"> The most recently selected trap. </param>
        /// <remarks> Subscribed to the <see cref="PlayerCharacter.OnTrapDeployed"/> event. </remarks>
        private void OnTrapDeployed(Trap trap) {
            if (UGS_Analytics.Instance is null) {
                return;
            }

            UGS_Analytics.DeployTrapCustomEvent(trap.gameObject.name);
        }

        /// <summary>
        /// Records analytics trap switching data.
        /// </summary>
        /// <param name="trap"> The most recently selected trap. </param>
        /// <remarks> Subscribed to the <see cref="PlayerCharacter.OnSelectedTrapChanged"/> event. </remarks>
        private void SelectedTrapIndexChanged(Trap trap) {
            if (UGS_Analytics.Instance is null) {
                return;
            }

            var isAffordable = ResourceManager.Instance.CanAffordCost(this._playerController.Player.GetTrapCost());
            UGS_Analytics.SwitchTrapCustomEvent(trap.gameObject.name, isAffordable);
        }

        #endregion

        /// <summary>
        /// Disables the player input and hero movement, destroying the player GameObject, and triggers the loss UI.
        /// </summary>
        /// <param name="message"> The message to be displayed on the loss UI. </param>
        /// <remarks> Helper for <see cref="GameManager.OnPlayerStateChanged"/>. </remarks>
        private void HenDied(string message) {
            // Invoke GameOverEvent before destroying Player gameObject
            this.HenLost(message);

            Destroy(this._playerController.gameObject);

            this.WaveManager.StopWaves();

            this._dialogueRunner.Stop();
        }

        /// <summary>
        /// Disables the player input and wave spawner, and disables the hero movement before triggering the win UI.
        /// </summary>
        /// <param name="endgameMessage"> The message to be displayed on the loss UI. </param>
        /// <remarks> Invokes the <see cref="OnHenLost"/> and <see cref="EventManager.GameOverEvent"/> events. </remarks>
        private void HenLost(string endgameMessage) {
            if (this._isGameOver) {
                return;
            }

            this._isGameOver = true;

            this.WaveManager.StopWaves();

            this.WaveManager.FreezeAllHeroes();
            this._playerController.StopSession();
            
            this.OnHenLost?.Invoke(endgameMessage);
            EventManager.Instance.GameOverEvent?.Invoke();
        }

        /// <summary>
        /// Disables and freezes player input and triggers the win UI. Updates the <see cref="DataManager"/> save
        /// data to reflect the completion of the current level and saves it to local storage.
        /// </summary>
        /// <remarks>
        /// Invokes the <see cref="OnHenWon"/> event.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, does nothing.
        /// Otherwise, stops recording the player input and creates a file for the recorded input. </p>
        /// </remarks>
        public void HenWon() {
            if (this._isGameOver) {
                return;
            }

            this._isGameOver = true;
            this._playerController.StopSession();
            
            this.OnHenWon?.Invoke("All heroes were defeated. Good job!");

            DataManager.Instance.PlayerData.AddCompletedLevel(this._levelData.Index);
            DataManager.Instance.SaveGameData();
        }

        /// <summary>
        /// Disables the player input and hero movement, and triggers the loss UI.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="EndgameTarget.OnHeroReachedEndgameTarget"/> event. </remarks>
        private void HeroReachedLevelEnd() {
            this.HenLost("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
        }

        

        /// <summary>
        /// Freezes the movement of all active actors within the level for a duration of time on a camera switch.
        /// </summary>
        /// <param name="freezeTime"> The duration of time to freeze active actor movement. </param>
        /// <remarks> Subscribed to the OnSwitchCameraFreeze GameEvent. </remarks>
        public void FreezeActors(float freezeTime) {
            StartCoroutine(this.FreezeAllActorsCoroutine(freezeTime));
        }

        /// <summary>
        /// Freezes the movement of the player and all heroes for a specified duration of time.
        /// </summary>
        /// <param name="freezeTime"> The duration of time to freeze movement. </param>
        private IEnumerator FreezeAllActorsCoroutine(float freezeTime) {
            this.WaveManager.FreezeAllHeroes();
            this._playerController.Player.FreezePosition();

            yield return new WaitForSeconds(freezeTime);
            
            this.WaveManager.UnfreezeAllHeroes();
            this._playerController.Player.UnfreezePosition();
        }

        #endregion
    }
}
