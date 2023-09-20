using KrillOrBeKrilled.Heatmaps;
using UnityEditor;
using UnityEngine;

//*******************************************************************************************
// HeatmapEditor
//*******************************************************************************************
namespace KrillOrBeKrilled.Editor {
    /// <summary>
    /// A subclass of <see cref="Editor"/> to draw extra buttons and fields in the inspector
    /// for <see cref="Heatmap"/>. Includes buttons for generating and clearing the heatmap
    /// data from the <see cref="CSVFile"/> and logic to autogenerate a file path from the
    /// <see cref="CSVFile"/> reference.
    /// </summary>
    [CustomEditor(typeof(Heatmap))]
    [CanEditMultipleObjects]
    public class HeatmapEditor : UnityEditor.Editor {
        SerializedProperty Heatmap;
        SerializedProperty CSVFile;
        SerializedProperty _filePath;
    
        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_filePath);
            
            DrawDefaultInspector();
    
            var heatmapScript = (Heatmap)target;
            if (GUILayout.Button("Generate Heatmap")) {
                heatmapScript.GenerateHeatmap();
            }
            
            if (GUILayout.Button("Clear Heatmap")) {
                heatmapScript.ClearHeatmap();
            }
    
            if (!CSVFile.objectReferenceValue) 
                return;
    
            _filePath.stringValue = AssetDatabase.GetAssetPath(CSVFile.objectReferenceValue.GetInstanceID());
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnEnable() {
            CSVFile = serializedObject.FindProperty("CSVFile");
            _filePath = serializedObject.FindProperty("_filePath");
        }
    }
}

