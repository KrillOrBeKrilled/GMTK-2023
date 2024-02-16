using System.IO;
using UnityEngine;
using Newtonsoft.Json;

// TODO: add saving to the TEMP file first, replace data file only upon completion
//*******************************************************************************************
// FileManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// A persistent file management class that provides methods to the game application
    /// to read and write to save data files.
    /// </summary>
    public static class FileManager {

        //========================================
        // Public Methods
        //========================================

        #region Public Methods
        
        /// <summary>
        /// Writes the data provided by the respective data class as a file with the specified file name on the
        /// local file storage of the device running the game application. Updates the file if it already exists.
        /// </summary>
        /// <param name="data"> Provides the data to be written to the local save files. </param>
        /// <param name="fileName"> The name of the file to be saved. </param>
        /// <typeparam name="T"> Represents any data class from the <see cref="KrillOrBeKrilled.Model"/> namespace. </typeparam>
        public static void SaveData<T>(T data, string fileName) {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }

        /// <summary>
        /// Retrieves the data from the locally stored save file and returns the data within the specified data
        /// class type. Reports the success or failure of this operation through the <c>out</c> parameter.
        /// </summary>
        /// <param name="fileName"> The name of the file to be loaded. </param>
        /// <param name="success"> Used to store data on whether or not the operation has succeeded. </param>
        /// <typeparam name="T"> Represents any data class from the <see cref="KrillOrBeKrilled.Model"/> namespace. </typeparam>
        /// <returns> The loaded data formatted as the specified T data class type. </returns>
        public static T LoadData<T>(string fileName, out bool success) {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(path)) {
                success = false;
                return default(T);
            }

            string jsonData = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(jsonData);
            success = true;
            return data;
        }
        
        #endregion
    }
}
