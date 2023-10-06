using UnityEditor;
using UnityEngine;

//*******************************************************************************************
// ForceReserialize
//*******************************************************************************************
namespace KrillOrBeKrilled.Editor {
    public class ForceReserialize : EditorWindow {
        [MenuItem("Window/ForceReserialize")]
        public static void ShowWindow() {
            GetWindow(typeof(ForceReserialize));
        }

        public void OnGUI() {
            if (GUILayout.Button("Force reserialize all assets.")) {
                PerformForceReserialize();
            }
        }

        private static void PerformForceReserialize() {
            AssetDatabase.ForceReserializeAssets();
            Debug.Log("Completed");
        }
    }
}
