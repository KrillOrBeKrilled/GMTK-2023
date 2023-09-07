using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers {
  public class SceneNavigationManager : Singleton<SceneNavigationManager> {
    public UnityEvent<Scene, LoadSceneMode> OnSceneLoaded { get; private set; }

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

    protected override void Awake() {
      base.Awake();
      this.OnSceneLoaded = new UnityEvent<Scene, LoadSceneMode>();
      DontDestroyOnLoad(this.gameObject);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode) {
      this.OnSceneLoaded?.Invoke(scene, mode);
    }

    private static void LoadScene(string sceneName) {
      SceneManager.LoadScene(sceneName);
    }

    private static void LoadScene(int sceneIndex) {
      SceneManager.LoadScene(sceneIndex);
    }

    private void OnEnable() {
      SceneManager.sceneLoaded += this.SceneLoaded;
    }

    private void OnDisable() {
      SceneManager.sceneLoaded -= this.SceneLoaded;
    }
  }
}
