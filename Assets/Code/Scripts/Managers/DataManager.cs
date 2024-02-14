using KrillOrBeKrilled.Model;
using UnityEngine.SceneManagement;

namespace KrillOrBeKrilled.Managers {
    public class DataManager : Singleton<DataManager> {
        public PlayerData PlayerData { get; private set; }

        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += this.OnSceneLoaded;
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
            this.LoadPlayerData();
        }

        private void OnApplicationQuit() {
            this.SaveGameData();
        }

        private void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus)
                return;

            this.SaveGameData();
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // Code to execute after a new scene is loaded
        }

        private void OnSceneUnloaded(Scene scene) {
            // Code to execute right before the current scene unloads
            this.SaveGameData();
        }

        public void SaveGameData() {
            this.SavePlayerData();
        }

        private void SavePlayerData() {
            FileManager.SaveData(this.PlayerData, "playerData.json");
        }

        private void LoadPlayerData() {
            this.PlayerData = FileManager.LoadData<PlayerData>("playerData.json", out bool success);
            if (!success)
                this.PlayerData = PlayerData.Default;
        }
    }
}
