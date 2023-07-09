using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : Singleton<SceneNavigationManager> {
  public void LoadGameScene() {
    LoadScene("Game");
  }

  public void ReloadCurrentScene() {
    LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void LoadMainMenuScene() {
    LoadScene("MainMenu");
  }

  public void ExitGame() {
    print("Exiting Game, Closing Application");
    Application.Quit();
  }

  private static void LoadScene(string sceneName) {
    SceneManager.LoadScene(sceneName);
  }

  private static void LoadScene(int sceneIndex) {
    SceneManager.LoadScene(sceneIndex);
  }
}
