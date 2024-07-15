using KrillOrBeKrilled.Model;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// DataManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Manages the saving and loading of persistent data files, acting as an intermediary
    /// between the game's core management system and the file management system.
    /// </summary>
    public class DataManager : Singleton<DataManager> {
        public PlayerData PlayerData { get; private set; }
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += this.OnSceneLoaded;
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
            this.LoadPlayerData();
        }
        
        private void OnDestroy() {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================

        #region Public Methods
        
        /// <summary> Saves the game data through the file management system. </summary>
        public void SaveGameData() {
            this.SavePlayerData();
        }
        
        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods
        
        /// <summary> Saves the game data upon quitting the application. </summary>
        private void OnApplicationQuit() {
            this.SaveGameData();
        }

        /// <summary> Saves the game data upon pausing the application. </summary>
        private void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus)
                return;

            this.SaveGameData();
        }
        
        /// <remarks> Subscribed to the <see cref="SceneManager.sceneLoaded"/> event. </remarks>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // Code to execute after a new scene is loaded
        }

        /// <summary> Saves the game data upon unloading the current scene. </summary>
        /// <remarks> Subscribed to the <see cref="SceneManager.sceneUnloaded"/> event. </remarks>
        private void OnSceneUnloaded(Scene scene) {
            // Code to execute right before the current scene unloads
            this.SaveGameData();
        }

        /// <summary> Notifies the <see cref="FileManager"/> to write the active game save data to file. </summary>
        private void SavePlayerData() {
            FileManager.SaveData(this.PlayerData, "playerData.json");
        }

        /// <summary>
        /// Notifies the <see cref="FileManager"/> to load the data read from the save file to the
        /// active game save data.
        /// </summary>
        private void LoadPlayerData() {
            this.PlayerData = FileManager.LoadData<PlayerData>("playerData.json", out bool success);
            bool isValid = PlayerData.IsValid(this.PlayerData);
            if (!success || !isValid) {
                this.PlayerData = PlayerData.Default;
            }
        }
        
        #endregion
    }
}
