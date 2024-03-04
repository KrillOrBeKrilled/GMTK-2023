using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace KrillOrBeKrilled.Editor {
  public class ScreenshotTool : EditorWindow {
    private Camera _targetCamera;
    
    [MenuItem("Henchman/Screenshot Tool")]
    public static void ShowWindow() {
      GetWindow<ScreenshotTool>("Screenshot Tool");
    }

    private void OnGUI() {
      GUILayout.Space(5);

      GUILayout.Label("Use this tool to capture the current view from the Camera.\n" +
                      "Captures at the resolution of the Game tab.");
      
      GUILayout.Space(5);
      
      this._targetCamera = EditorGUILayout.ObjectField("Target Camera", this._targetCamera, 
                                                       typeof(Camera), true) as Camera;
      
      GUILayout.Space(15);
      if (GUILayout.Button("Capture Scene")) {
        this.StartCoroutine(this.CaptureScene());
      }
        
      if (GUILayout.Button("Delete All Screenshots")) {
        this.StartCoroutine(ClearScreenshots());
      }
    }

    private static string GetPath() {
      string res = $"{Screen.width}x{Screen.height}";
      string folderPath = Path.Combine(Application.dataPath, "Screenshots", res);
      Directory.CreateDirectory(folderPath);
      int index = Directory.GetFiles(folderPath).Length;
      string filePath = Path.Combine(folderPath, $"{res}_{index}.png");
      Debug.Log(filePath);
      return filePath;
    }
    
    private static string GetFolderPath() {
      string folderPath = Path.Combine(Application.dataPath + "/Screenshots");
      Directory.CreateDirectory(folderPath);
      return folderPath;
    }

    private static IEnumerator ClearScreenshots() {
      bool confirmation = EditorUtility.DisplayDialog("Delete All Screenshots",
                                                      "Delete previously saved screenshots?",
                                                      "OK",
                                                      "Cancel");
      if (!confirmation) {
        yield break;
      }

      string screenshotsFolder = GetFolderPath();
      string[] files = Directory.GetFiles(screenshotsFolder);
      foreach (string file in files) {
        File.Delete(file);
      }
        
      Debug.Log("All screenshots deleted.");
      yield return null;
      AssetDatabase.Refresh();
    }

    private IEnumerator CaptureScene() {
      if (this._targetCamera == null) {
        yield break;
      }
      
      // Save the screenshot
      string path = GetPath();
      ScreenCapture.CaptureScreenshot(path);
      Debug.Log($"Screenshot saved to: {path}");
      
      yield return new EditorWaitForSeconds(0.5f);
      AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
  }
}