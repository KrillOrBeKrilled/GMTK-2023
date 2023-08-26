using Heatmaps;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Heatmap))]
[CanEditMultipleObjects]
public class HeatmapEditor : Editor
{
    SerializedProperty Heatmap;
    SerializedProperty CSVFile;
    SerializedProperty _filePath;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_filePath);
        
        DrawDefaultInspector();

        var heatmapScript = (Heatmap)target;
        if (GUILayout.Button("Generate Heatmap"))
        {
            heatmapScript.GenerateHeatmap();
        }
        
        if (GUILayout.Button("Clear Heatmap"))
        {
            heatmapScript.ClearHeatmap();
        }

        if (!CSVFile.objectReferenceValue) return;

        _filePath.stringValue = AssetDatabase.GetAssetPath(CSVFile.objectReferenceValue.GetInstanceID());
        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnEnable()
    {
        CSVFile = serializedObject.FindProperty("CSVFile");
        _filePath = serializedObject.FindProperty("_filePath");
    }
}
