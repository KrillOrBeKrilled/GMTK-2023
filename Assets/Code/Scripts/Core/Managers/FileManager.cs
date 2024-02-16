using System.IO;
using UnityEngine;
using Newtonsoft.Json;

// TODO: add saving to the TEMP file first, replace data file only upon completion
namespace KrillOrBeKrilled.Core.Managers {
    public static class FileManager {

        public static void SaveData<T>(T data, string fileName) {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }

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
    }
}
