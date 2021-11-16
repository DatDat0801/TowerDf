using System;
using System.Collections.Generic;

namespace EW2
{
    /// <summary>
    /// Define tower stats of user
    /// </summary>
    [Serializable]
    public class UserTowerData
    {
        public List<TowerStat> towerStats;

        public UserTowerData()
        {
            towerStats = new List<TowerStat>();
        }
        public class TowerStat
        {
            public int towerId;
            public int towerLevel;
        }

        public void Upgrade(int towerId)
        {
            if (towerStats == null)
            {
                towerStats = new List<TowerStat>();
            }
            
            var index = towerStats.FindIndex(stat => stat.towerId == towerId);
            if (index != -1)
            {
                var p = towerStats[index];
                p.towerLevel += 1;
                //return p.towerLevel;
            }
            else
            {
                var newTower = new TowerStat() {towerId = towerId, towerLevel = 1};
                towerStats.Add(newTower);
                //return newTower.towerLevel;
            }
        }

        public TowerStat GetTowerStat(int towerId)
        {
            if (towerStats == null)
            {
                towerStats = new List<TowerStat>();
                return null;
            }

            return towerStats.Find(stat => stat.towerId == towerId);
        }

        public int GetCurrentLevel(int towerId)
        {
            var stat = GetTowerStat(towerId);
            if (stat == null)
            {
                return 0;
            }
            return stat.towerLevel;
        }

        public void ResetUserTowerData()
        {
            towerStats.Clear();
        }

        public bool CanResetProgress()
        {
            if (towerStats == null) return false;
            if (towerStats.Count <= 0) return false;

            return true;
        }
    }
}