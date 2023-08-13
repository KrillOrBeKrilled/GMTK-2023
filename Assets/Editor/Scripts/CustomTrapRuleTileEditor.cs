using UnityEngine;
using UnityEditor;

//*******************************************************************************************
// CustomTrapRuleTileEditor
//*******************************************************************************************
/// <summary>
/// A subclass of RuleTileEditor that provides custom sprites for the
/// CustomTrapRuleTile class neighboring tile rule visualizer in the inspector.
/// </summary>
[CustomEditor(typeof(CustomTrapRuleTile))]
[CanEditMultipleObjects]
public class CustomTrapRuleTileEditor : RuleTileEditor
{
    public Texture2D ground, dungeon;

    public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
    {
        switch (neighbor)
        {
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
