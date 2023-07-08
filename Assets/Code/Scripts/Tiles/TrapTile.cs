using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrapTile : Tile 
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom Tiles/Trap Tile")]
    public static void CreateTrapTile()
    {
        var path = EditorUtility.SaveFilePanelInProject("Save Trap Tile", "New Trap Tile",
            "Asset", "Save Trap Tile", "Assets");
        if (path == "") return;
        
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TrapTile>(), path);
    }
    #endif
}