using KrillOrBeKrilled.Common;
using UnityEditor;
using UnityEngine;

namespace KrillOrBeKrilled {
  [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
  public class EventEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      GUI.enabled = Application.isPlaying;

      GameEvent e = target as GameEvent;
      if (GUILayout.Button("Raise"))
        e.Raise();
    }
  }
}