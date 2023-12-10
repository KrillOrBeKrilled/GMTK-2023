using KrillOrBeKrilled.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// MainMenu
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the fading transitions of the MainMenu scene, along with loading the
    /// levels scene through the <see cref="SceneNavigationManager"/>.
    /// </summary>
    public class MainMenu : MonoBehaviour {
        [Tooltip("Used to fade the scene in and out.")]
        [SerializeField] private Image _foreground;

        private const float FadeDuration = 0.5f;
        private const string FormURL = "https://forms.gle/jjx4hbcQJiN9eDLk9";

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
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
        /// Fades in the screen and loads the Levels scene upon completion.
        /// </summary>
        public void OnPlay() {
            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(1, FadeDuration)
                .OnComplete(SceneNavigationManager.Instance.LoadLevelsScene);
        }

        public void GoToFeedbackForm() {
            Application.OpenURL(FormURL);
        }

        #endregion
    }
}
