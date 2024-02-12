using System;
using System.Collections.Generic;

namespace KrillOrBeKrilled.Model {
    [Serializable]
    public class PlayerData {
        public HashSet<int> CompletedLevels = new HashSet<int>();

        public static PlayerData Default => new PlayerData {
            CompletedLevels = new HashSet<int>()
        };

        public void AddCompletedLevel(int levelIndex) {
            if (levelIndex <= 0)
                return;

            this.CompletedLevels.Add(levelIndex);
        }
    }
}
