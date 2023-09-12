using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
    [CreateAssetMenu(fileName = "LevelDictionary", menuName = "LevelDictionary")]
    public class LevelsList : ScriptableObject {
        public List<LevelData> LevelDatas;
    }
}
