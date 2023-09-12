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
    /// game scene through the <see cref="SceneNavigationManager"/>.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Tooltip("Used to fade the scene in.")] [SerializeField]
        private Image _foreground;

        private const float FadeDuration = 0.5f;

        /// <summary> Fades in the screen and loads the game scene upon completion. </summary>
        public void OnStartGame()
        {
            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(1, FadeDuration)
                .OnComplete(SceneNavigationManager.Instance.LoadGameScene);
        }

        private void Awake()
        {
            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(0, FadeDuration)
                .OnComplete(() => this._foreground.gameObject.SetActive(false));
        }

        public void OnPlay()
        {
            this._foreground.gameObject.SetActive(true);
            this._foreground
                .DOFade(1, FadeDuration)
                .OnComplete(SceneNavigationManager.Instance.LoadLevelsScene);
        }
    }
}