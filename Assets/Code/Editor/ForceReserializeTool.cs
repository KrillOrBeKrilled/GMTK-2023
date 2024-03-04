using UnityEditor;
using UnityEngine;

//*******************************************************************************************
// ForceReserialize
//*******************************************************************************************
namespace KrillOrBeKrilled.Editor {
    public static class ReserializeAllAssets {
        [MenuItem("Henchman/Force Reserialize All Assets")]
        public static void ForceReserializeAllAssets() {
            if (EditorUtility.DisplayDialog("Force Reserialize All Assets",
                                            "This will re-serialize all assets and might take some time. Proceed?",
                                            "Yes", "No")) {
                string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
                
                Debug.Log("Starting re-serialization of all assets...");
                AssetDatabase.ForceReserializeAssets();
                Debug.Log($"Re-serialization complete. {allAssetPaths.Length} assets re-serialized.");
            }
        }
    }
    
    public static class ForceReserializeAssetsUtility {
        [MenuItem("Henchman/Force Reserialize Selected", true)]
        private static bool ForceReserializeAssetsValidation() {
            return Selection.assetGUIDs.Length > 0;
        }

        [MenuItem("Henchman/Force Reserialize Selected")]
        private static void ForceReserializeSelectedAssets() {
            string[] selectedAssetPaths = Selection.assetGUIDs;

            for (int i = 0; i < selectedAssetPaths.Length; i++) {
                selectedAssetPaths[i] = AssetDatabase.GUIDToAssetPath(selectedAssetPaths[i]);
            }

            AssetDatabase.ForceReserializeAssets(selectedAssetPaths);
            Debug.Log($"Forced re-serialization on {selectedAssetPaths.Length} assets.");
        }
    }
}
