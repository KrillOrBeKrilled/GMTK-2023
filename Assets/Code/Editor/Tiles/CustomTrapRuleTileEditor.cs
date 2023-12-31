using KrillOrBeKrilled.Tiles;
using UnityEngine;
using UnityEditor;

//*******************************************************************************************
// CustomTrapRuleTileEditor
//*******************************************************************************************
namespace KrillOrBeKrilled.Editor {
    /// <summary>
    /// A subclass of <see cref="RuleTileEditor"/> that provides custom sprites for the
    /// CustomTrapRuleTile class neighboring tile rule visualizer in the inspector.
    /// </summary>
    [CustomEditor(typeof(CustomTrapRuleTile))]
    [CanEditMultipleObjects]
    public class CustomTrapRuleTileEditor : RuleTileEditor {
        [Tooltip("Sprites to signify the extended rule tile rules.")]
        public Texture2D ground, dungeon;
    
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
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
        
        #endregion
    }
}

