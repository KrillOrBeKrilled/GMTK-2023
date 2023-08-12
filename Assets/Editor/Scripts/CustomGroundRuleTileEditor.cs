using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomGroundRuleTile))]
[CanEditMultipleObjects]
public class CustomGroundRuleTileEditor : RuleTileEditor
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
