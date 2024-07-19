using KrillOrBeKrilled.Model;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrillOrBeKrilled.Editor {
  public class LevelEditorWindow : EditorWindow {
    private LevelData currentLevelData;
    private GameObject levelEndPrefab;
    private int selectedTab = 0;
    private string[] tabNames = { "LevelData Editor", "Settings" };
    private const string LevelEndPrefabPathKey = "LevelEditor_LevelEndPrefabPath";

    [MenuItem("Henchman/Level Editor")]
    public static void ShowWindow() {
      GetWindow<LevelEditorWindow>("Level Editor");
    }
      
    void OnEnable()
    {
      // Load the levelEndPrefab based on the saved asset path
      string path = EditorPrefs.GetString(LevelEndPrefabPathKey, "");
      if (!string.IsNullOrEmpty(path))
      {
        levelEndPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
      }
    }
    
    private void OnGUI() {
      // Display tabs
      EditorGUILayout.Space();
      selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
      EditorGUILayout.Space();
      // Display controls based on selected tab
      switch (selectedTab)
      {
        case 0:
          this.LevelEditorTab();
          break;
        case 1:
          this.SettingsTab();
          break;
        default:
          break;
      }
    }
    
    private void LevelEditorTab()
    {
      GUILayout.Label("Level Data", EditorStyles.boldLabel);
      currentLevelData = EditorGUILayout.ObjectField("Level Data", currentLevelData, typeof(LevelData), false) as LevelData;
      
      if (currentLevelData != null)
      {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Edit Level"))
        {
          EditLevel();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Save Level"))
        {
          SaveLevel();
        }
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
      }
    }

    private void SettingsTab()
    {
      EditorGUILayout.LabelField("Settings");
      GameObject newLevelEndPrefab = EditorGUILayout.ObjectField("Level End Prefab", levelEndPrefab, typeof(GameObject), false) as GameObject;
      if (newLevelEndPrefab != levelEndPrefab)
      {
        levelEndPrefab = newLevelEndPrefab;
        if (levelEndPrefab != null)
        {
          string path = AssetDatabase.GetAssetPath(levelEndPrefab);
          EditorPrefs.SetString(LevelEndPrefabPathKey, path);
        }
        else
        {
          EditorPrefs.DeleteKey(LevelEndPrefabPathKey);
        }
      }
    }
    
    void EditLevel()
    {
      // Load or create a temporary scene or prefab view
      // Assuming we use a temporary scene:
      var tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
      SceneManager.SetActiveScene(tempScene);

      // Instantiate the tilemap
      if (currentLevelData.WallsTilemapPrefab)
        PrefabUtility.InstantiatePrefab(currentLevelData.WallsTilemapPrefab, tempScene);

      // Spawn level end
      if (currentLevelData.WallsTilemapPrefab)
      {
        var levelEnd = PrefabUtility.InstantiatePrefab(levelEndPrefab, tempScene) as GameObject;
        levelEnd.transform.position = currentLevelData.EndgameTargetPosition;
      }

      // Spawn start position placeholder
      // GameObject startPositionMarker = new GameObject("StartPosition");
      // startPositionMarker.transform.position = currentLevelData.startPosition;
    }

    private void SaveLevel()
    {
      // You would iterate through the objects in the scene, saving their relevant data
      // Here, for simplicity, we assume the start and end positions might be adjusted in the editor

      // Find objects in the scene and save their positions back to the LevelData
      var levelEnd = GameObject.Find("LevelEnd");
      if (levelEnd)
        currentLevelData.EndgameTargetPosition = levelEnd.transform.position;

      // var startPositionMarker = GameObject.Find("StartPosition");
      // if (startPositionMarker)
      //   currentLevelData.startPosition = startPositionMarker.transform.position;

      EditorUtility.SetDirty(currentLevelData);
      AssetDatabase.SaveAssets();
    }

    private void LoadLevel()
    {
      // Ensure there is a valid level data to load
      if (currentLevelData == null)
      {
        Debug.LogError("No level data selected to load!");
        return;
      }

      // Load the current level data asset
      // You don't need to do anything here since the currentLevelData is already set
      Debug.Log("Level loaded: " + currentLevelData.name);
    }
  }
}