using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// SceneNavigationManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Communicates directly with <see cref="SceneManager"/> to load scenes and exit
    /// the application through exposed methods.
    /// </summary>
    public class SceneNavigationManager : Singleton<SceneNavigationManager> {
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Exits the application.
        /// </summary>
        public void ExitGame() {
            print("Exiting Game, Closing Application");
            Application.Quit();
        }
        
        /// <summary>
        /// Loads the Game scene.
        /// </summary>
        public static void LoadGameLevelScene() {
            LoadScene("Game");
        }
        
        /// <summary>
        /// Loads the "Lobby" scene.
        /// </summary>
        public static void LoadLobbyScene() {
            LoadScene("Lobby");
        }
        
        /// <summary>
        /// Loads the "MainMenu" scene.
        /// </summary>
        public static void LoadMainMenuScene() {
            LoadScene("MainMenu");
        }

        /// <summary>
        /// Loads the current scene.
        /// </summary>
        public static void ReloadCurrentScene() {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Helper method for loading a scene by index.
        /// </summary>
        /// <param name="sceneIndex"> The index of the scene to load. </param>
        private static void LoadScene(int sceneIndex) {
            DOTween.KillAll();
            SceneManager.LoadScene(sceneIndex);
        }
        
        /// <summary>
        /// Helper method for loading a scene by name.
        /// </summary>
        /// <param name="sceneName"> The name of the scene to load. </param>
        private static void LoadScene(string sceneName)
        {
            DOTween.KillAll();
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
