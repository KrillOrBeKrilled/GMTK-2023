using System;
using System.Collections.Generic;

namespace KrillOrBeKrilled.Model {
    [Serializable]
    public class PlayerData {
        public HashSet<int> CompletedLevels = new HashSet<int>();

        public static PlayerData Default => new PlayerData {
            CompletedLevels = new HashSet<int>()
        };

        public static bool IsValid(PlayerData data) {
            if (data == null) return false;
            if (data.CompletedLevels == null) return false;


            return true;
        }

        public void AddCompletedLevel(int levelIndex) {
            if (levelIndex <= 0)
                return;

            this.CompletedLevels.Add(levelIndex);
        }
    }
}
