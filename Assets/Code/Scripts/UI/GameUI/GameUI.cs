using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Core.Managers;
using KrillOrBeKrilled.Heroes;
using System.Collections.Generic;
using KrillOrBeKrilled.Model;
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
    /// <remarks> Certain elements such as UI initialization and screen wipe-in effects
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
        [Tooltip("WavesUI")] 
        [SerializeField] private WavesUI _wavesUI;
        [Tooltip("The coin counter UI text displayed during gameplay.")]
        [SerializeField] private TMP_Text _coinsText;
        [Tooltip("The trap selector toolbar displayed during gameplay.")]
        [SerializeField] private TrapSelectionBar _trapSelectionBar;
        [Tooltip("The Skip Dialogue HUD displayed during dialogue sequences.")]
        [SerializeField] private SkipDialogueUI _skipDialogueUI;
        [Tooltip("Holds all references to the hero health bars as their parent object.")]
        [SerializeField] private Transform _healthBarsContainer;
        [Tooltip("The hero and player progress to the level goal tracker Map UI displayed during gameplay.")]
        [SerializeField] private MapUI _mapUI;
        [Tooltip("The on-screen buttons used to control the player displayed during gameplay.")]
        [SerializeField] private ControlsUI _controlsUI;
        [Tooltip("The trap resource inventory widget displayed during gameplay.")]
        [SerializeField] private ResourceUI _resourceUI;
        [Tooltip("Resource Group to be hidden or shown.")] 
        [SerializeField] private ResourceUIGroup _resourceUIGroup;
        [Tooltip("The trap resource requirements widget displayed during gameplay.")]
        [SerializeField] private TrapRequirementsUI _trapRequirementsUI;

        [Header("Prefabs")]
        [Tooltip("The hero health bar to instantiate upon spawning a new hero.")]
        [SerializeField] private HealthBarUI _healthBarUIPrefab;

        private UnityAction _onScreenWipeInComplete;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Start() {
            this._gameManager.OnHenWon.AddListener(this.OnHenWon);
            this._gameManager.OnHenLost.AddListener(this.OnHenLost);
            this._gameManager.WaveManager.OnHeroSpawned.AddListener(this.OnHeroSpawned);
            this._gameManager.OnSceneWillChange.AddListener(this.ScreenWipeInSceneCover);
            
            this._resourceUIGroup.Initialize(this._gameManager.ShouldHideResourceUI);

            this._trapRequirementsUI.Initialize(this._gameManager.Player.OnSelectedTrapChanged);
            this._trapSelectionBar.Initialize(
                this._gameManager.Player.OnSelectedTrapChanged,
                this._gameManager.TrapController.Traps,
                this._gameManager.Player.SetTrap
            );
            
            this._mapUI.Initialize(
                this._gameManager.PlayerController.transform,
                this._gameManager.LevelStart.x,
                this._gameManager.LevelEnd.position.x
            );

            this._controlsUI.Initialize(this._gameManager.Player);
            this._wavesUI.Initialize(this._gameManager.IsEndless);

            EventManager.Instance.CoinAmountChangedEvent.AddListener(this.OnCoinsUpdated);
            EventManager.Instance.PauseToggledEvent.AddListener(this.OnPauseToggled);
            EventManager.Instance.ResourceAmountChangedEvent.AddListener(this.OnResourceUpdate);
        }
        
        /// <summary>
        /// Plays a screen wipe-out transition effect.
        /// </summary>
        /// <remarks> Listens on the <b>OnSetupComplete</b> <see cref="GameEvent"/> event. </remarks>
        public void OnGameSetupComplete() {
            this._screenWipe.SetRandomWipeShape();
            this._loadingScreen.gameObject.SetActive(false);
            this._screenWipe.WipeOut();
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods
        
        /// <summary>
        /// Enables the GameObject that controls the loading screen and invokes the
        /// <see cref="_onScreenWipeInComplete"/> function.
        /// </summary>
        /// <remarks> Triggered by <see cref="ScreenWipeUI.WipeIn"/> upon completion. </remarks>
        private void CompleteSceneChange() {
            this._loadingScreen.gameObject.SetActive(true);
            this._onScreenWipeInComplete?.Invoke();
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
        /// Updates the text on the resource UI for a specific resource Type.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="ResourceAmountChangedEvent"/> event. </remarks>
        private void OnResourceUpdate(Dictionary<ResourceType, int> resources) {
            foreach (var resource in resources) {
                this._resourceUI.SetAmount(resource.Key, resource.Value);
            }
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
        /// Creates a health bar for the associated <see cref="Hero"/>, registers the hero to be represented
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
        /// Plays a screen wipe-in transition effect and sets a function to invoke upon completion.
        /// </summary>
        /// <param name="onComplete"> The function to invoke once the screen wipe-in effect has been completed. </param>
        private void ScreenWipeInSceneCover(UnityAction onComplete) {
            this._onScreenWipeInComplete = onComplete;

            this._screenWipe.gameObject.SetActive(true);
            this._screenWipe.SetRandomWipeShape();
            this._screenWipe.WipeIn(this.CompleteSceneChange);
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
