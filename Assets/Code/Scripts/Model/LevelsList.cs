using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************
// LevelsList
//*******************************************************************************************
namespace KrillOrBeKrilled.Model {
    /// <summary>
    /// Stores data on all <see cref="LevelData"/> levels in a set.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelDictionary", menuName = "LevelDictionary")]
    public class LevelsList : ScriptableObject {
        public List<LevelData> LevelDatas;
    }
}
