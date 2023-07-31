using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Heatmap))]
[CanEditMultipleObjects]
public class HeatmapEditor : Editor
{
    SerializedProperty Heatmap;

    public override void OnInspectorGUI()
    {
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
    }
}
