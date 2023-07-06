using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : Singleton<SceneNavigationManager> {
  public void LoadGameScene() {
    this.LoadScene("Game");
  }

  public void LoadMainMenuScene() {
    this.LoadScene("MainMenu");
  }

  public void ExitGame() {
    print("Exiting Game, Closing Application");
    Application.Quit();
  }

  public void LoadScene(string sceneName) {
    SceneManager.LoadScene(sceneName);
  }
}
