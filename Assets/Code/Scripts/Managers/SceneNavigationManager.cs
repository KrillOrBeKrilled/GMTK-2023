using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// SceneNavigationManager
//*******************************************************************************************
namespace Managers {
    /// <summary>
    /// Communicates directly with <see cref="SceneManager"/> to load scenes and exit
    /// the application through exposed methods.
    /// </summary>
    public class SceneNavigationManager : Singleton<SceneNavigationManager> {
        /// <summary>
        /// Loads the "Game" scene.
        /// </summary>
        public void LoadGameScene() {
            LoadScene("Game");
        }

        /// <summary>
        /// Loads the current scene.
        /// </summary>
        public void ReloadCurrentScene() {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Loads the "MainMenu" scene.
        /// </summary>
        public void LoadMainMenuScene() {
            LoadScene("MainMenu");
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void ExitGame() {
            print("Exiting Game, Closing Application");
            Application.Quit();
        }

        /// <summary>
        /// Helper method for loading a scene by name.
        /// </summary>
        /// <param name="sceneName"> The name of the scene to load. </param>
        private static void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Helper method for loading a scene by index.
        /// </summary>
        /// <param name="sceneIndex"> The index of the scene to load. </param>
        private static void LoadScene(int sceneIndex) {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
