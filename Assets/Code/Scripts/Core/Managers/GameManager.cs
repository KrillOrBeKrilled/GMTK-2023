using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Common;
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
        [SerializeField] private DialogueManager _dialogueManager;
        [field:SerializeField] public WaveManager WaveManager { get; private set; }
        
        
        [Header("Level")]
        [SerializeField] private EndgameTarget _endgameTargetPrefab;
        [SerializeField] private Transform _tilemapGrid;

        [Header("Testing")]
        [Tooltip("Assign if you want to test specific level. Important: Leave as NULL when done testing.")]
        [SerializeField] private LevelData _testingLevelData;


        public PlayerController PlayerController => this._playerController;
        public PlayerCharacter Player => this._playerController.Player;
        public TrapController TrapController => this._playerController.TrapController;

        public Vector3 LevelStart { get; private set; }
    
        public Transform LevelEnd => this._endgameTarget.transform;

        /// The instantiated endgameTargetPrefab.
        private EndgameTarget _endgameTarget;

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

        public List<Sprite> ComicPages => this._levelData.ComicPages.AsReadOnly().ToList();
        
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
            this._pauseManager.SetIsPausable(true);
            
            bool isStoryLevel = this._levelData.Type == LevelData.LevelType.Story;
            bool shouldSkip = PlayerPrefsManager.ShouldSkipDialogue();
            
            if (!isStoryLevel || shouldSkip) {
                this.StartLevel();
                yield break;
            }
            
            // TODO: REPLACE WITH STRING EVENT
            this._dialogueManager.StartDialogue(this._levelData.DialogueName, this.ComicPages);
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

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
            LevelData.CopyData(sourceData, ref this._levelData);
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
            this._cameraManager.SetupCameras(this._levelData.StartCameraPosition, this._levelData.EndCameraPosition);
            this._cameraManager.SetBounds(this._levelData.WallsTilemapPrefab.transform.GetComponentExactlyInChildren<Collider2D>());
            this.LevelStart = this._levelData.RespawnPositions.First();
        }
        
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

        public void StartLevel() {
            this._playerController.StartSession();
            CoinManager.Instance.StartCoinEarning();
            ResourceSpawner.Instance.StartSpawner();
            this.WaveManager.StartWaveSpawning();
            this._onStartLevel.Raise();
        }

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
            this._dialogueManager.StopDialogue();
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
