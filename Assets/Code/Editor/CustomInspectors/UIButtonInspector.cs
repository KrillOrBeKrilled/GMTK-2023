using UnityEditor;
using UnityEngine;
using KrillOrBeKrilled.UI;

namespace KrillOrBeKrilled.Editor {
  [CustomEditor(typeof(UIButton))]
  public class UIButtonInspector : UnityEditor.Editor {
    
    //========================================
    // Unity Methods
    //========================================
    
    public override void OnInspectorGUI() {
      this.DrawDefaultInspector();
      
      UIButton uiButton = (UIButton)this.target;

      // Add a button if button has no targets
      if (uiButton.TargetImageCount <= 0) {
        if (GUILayout.Button("Add Target Image")) {
          this.AttachButtonTarget(uiButton);
        }
      }

      if (GUI.changed)
      {
        EditorUtility.SetDirty(this.target);
      }
    }
    
    //========================================
    // Private Methods
    //========================================

    /// <summary>
    /// Attaches a <see cref="UIButtonTarget"/> to the provided UIButton.
    /// </summary>
    /// <param name="uiButton">The UIButton to attach the UIButtonTarget to. </param>
    private void AttachButtonTarget(UIButton uiButton) {
      UIButtonTarget buttonTarget = uiButton.gameObject.AddComponent<UIButtonTarget>();
      uiButton.AddTarget(buttonTarget);
    }
  }
}