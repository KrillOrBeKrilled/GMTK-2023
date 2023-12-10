using DG.Tweening;
using KrillOrBeKrilled.Managers;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// LevelsUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the fading transitions of the Levels scene, along with loading the
    /// game scene through the <see cref="SceneNavigationManager"/>.
    /// </summary>
    public class LevelsUI : MonoBehaviour {
        private string _levelNameToLoad;

        [Tooltip("Used to fade the scene in and out.")]
        [SerializeField] private Image _foreground;
        [Tooltip("Used to cover the scene until the next level loads.")]
        [SerializeField] private Image _loadingScreen;
        [Tooltip("Used to screen wipe the scene in and out.")]
        [SerializeField] private ScreenWipeUI _screenWipe;

        private const float FadeDuration = 0.5f;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            this._screenWipe.gameObject.SetActive(false);

            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(0, FadeDuration)
                .OnComplete(() => this._foreground.gameObject.SetActive(false));
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Plays a screen wipe-in transition effect and sets the level to load upon completion.
        /// </summary>
        /// <param name="levelName"> The name of the level corresponding to the LevelData name. </param>
        public void LoadLevel(string levelName) {
            this._levelNameToLoad = levelName;

            this._screenWipe.gameObject.SetActive(true);
            this._screenWipe.SetRandomWipeShape();
            this._screenWipe.WipeIn(this.LoadLevelScene);
        }

        /// <summary>
        /// Fades in the screen and loads the MainMenu scene upon completion.
        /// </summary>
        public void LoadMainMenu() {
            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(1, FadeDuration)
                .OnComplete(SceneNavigationManager.Instance.LoadMainMenuScene);
        }

        #endregion

        //========================================
        // Internal Methods
        //========================================

        #region Internal Methods

        /// <summary>
        /// Loads the level corresponding to <see cref="_levelNameToLoad"/>.
        /// </summary>
        /// <remarks> Triggered by <see cref="ScreenWipeUI.WipeIn"/> upon completion. </remarks>
        internal void LoadLevelScene() {
            this._loadingScreen.gameObject.SetActive(true);
            LevelManager.Instance.LoadLevel(this._levelNameToLoad);
        }

        #endregion
    }
}
