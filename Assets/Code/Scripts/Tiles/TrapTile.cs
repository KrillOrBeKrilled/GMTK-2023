using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    //*******************************************************************************************
    // TrapTile
    //*******************************************************************************************
    /// <summary>
    /// A subclass of Tile that takes on the same functionality, only to be used as a type
    /// specifier for the trap deployment validation score in the PlayerController class.
    /// </summary>
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
            
            AssetDatabase.CreateAsset(CreateInstance<TrapTile>(), path);
        }
    #endif
    }
}
