using Player;
using UnityEditor;

//*******************************************************************************************
// RecordingControllerEditor
//*******************************************************************************************
/// <summary>
/// A subclass of <see cref="Editor"/> to autogenerate a file path from the
/// <see cref="RecordingFile"/> reference for <see cref="RecordingController"/>.
/// </summary>
[CustomEditor(typeof(RecordingController))]
[CanEditMultipleObjects]
public class RecordingControllerEditor : Editor {
    SerializedProperty RecordingFile;
    SerializedProperty _filePath;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_filePath);
        
        DrawDefaultInspector();

        if (!RecordingFile.objectReferenceValue) 
            return;

        _filePath.stringValue = AssetDatabase.GetAssetPath(RecordingFile.objectReferenceValue.GetInstanceID());
        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnEnable() {
        RecordingFile = serializedObject.FindProperty("RecordingFile");
        _filePath = serializedObject.FindProperty("_filePath");
    }
}
