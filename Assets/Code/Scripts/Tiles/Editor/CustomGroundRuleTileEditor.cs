using UnityEngine;
using UnityEditor;

//*******************************************************************************************
// CustomGroundRuleTileEditor
//*******************************************************************************************
namespace KrillOrBeKrilled.Tiles {
    /// <summary>
    /// A subclass of <see cref="RuleTileEditor"/> that provides custom sprites for the
    /// CustomGroundRuleTile class neighboring tile rule visualizer in the inspector.
    /// </summary>
    [CustomEditor(typeof(CustomGroundRuleTile))]
    [CanEditMultipleObjects]
    public class CustomGroundRuleTileEditor : RuleTileEditor {
        [Tooltip("Sprites to signify the extended rule tile rules.")]
        public Texture2D ground, dungeon;
    
        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor) {
            switch (neighbor) {
                case 3:
                    GUI.DrawTexture(rect, ground);
                    return;
                case 4:
                    GUI.DrawTexture(rect, dungeon);
                    return;
            }
            
            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}

