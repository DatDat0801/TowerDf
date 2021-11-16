using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public sealed class TowerUpgradeTool
    {
        private const int NUMBER_OF_TOWER = 4;

        public static void Upgrade2001()
        {
            //var towerUpgrade = ResourceUtils.Get<TowerUpgradeData>("Assets/EW2/Resources/CSV/UpgradeTowerSystem/");

            UserData.Instance.UpgradeTower(2001);
        }

        public static bool IsEnoughIngredient(int need, int starId)
        {
            var remainingStar = GetRemainingStar(starId);
            return need <= remainingStar;
        }

        public static int GetConsumedStars(int starId)
        {
            try
            {
                var userData = UserData.Instance.UserTowerData;
                var upgradeData = GameContainer.Instance.GetTowerUpgradeData();
                int result = 0;
                for (var i = 0; i < userData.towerStats.Count; i++)
                {
                    for (int j = 0; j < userData.towerStats[i].towerLevel; j++)
                    {
                        if (starId == MoneyType.GoldStar && j >= 3)
                        {
                            result += upgradeData.GetStarQuantity(starId, userData.towerStats[i].towerId, j + 1);
                        }
                        else if (starId == MoneyType.SliverStar && j < 3)
                        {
                            result += upgradeData.GetStarQuantity(starId, userData.towerStats[i].towerId, j + 1);
                        }

                        // result += upgradeData.GetStarQuantity(starId, userData.towerStats[i].towerId,
                        //     starId == MoneyType.SliverStar ? j + 1 : j + 4);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return 0;
            }
        }

        public static int GetRemainingStar(int starId)
        {
            var userData = UserData.Instance.CampaignData;
            return userData.GetCollectedStars(starId);

        }

        public static int GetNeededStar(int moneyType)
        {
            var upgradeData = GameContainer.Instance.GetTowerUpgradeData();
            return upgradeData.GetNeededStar(moneyType);
        }

        public static int GetNeededStarForLevel(int towerId, int towerLevel, int starId)
        {
            var towerUpgradeData = GameContainer.Instance.GetTowerUpgradeData();
            var quantity = towerUpgradeData.GetStarQuantity(starId, towerId, towerLevel);
            return quantity;
        }

        public static bool IsActivated(int towerId, int towerLevel)
        {
            var userData = UserData.Instance.UserTowerData;
            var index = userData.towerStats.FindIndex(s => s.towerId == towerId);
            if (index == -1)
            {
                return false;
            }

            var stat = userData.towerStats[index];
            return towerLevel <= stat.towerLevel;
        }

        public static bool CanUpgradeSomething(int starId)
        {
            try
            {
                var upgradeData = GameContainer.Instance.GetTowerUpgradeData();
                var userData = UserData.Instance.UserTowerData;
                if (userData == null) return false;
                var currentStar = GetRemainingStar(starId);
                //case userData has tower id
                List<int> checkedTowerIds = new List<int>();
                int canUpgradeCases = 0;
                if (userData.towerStats != null)
                {
                    int countTowerMaxLevel = 0;
                    foreach (var userDataTowerStat in userData.towerStats)
                    {
                        checkedTowerIds.Add(userDataTowerStat.towerId);

                        if (userDataTowerStat.towerLevel < 6)
                        {
                            var neededQuantity = upgradeData.GetStarQuantity(starId,
                                userDataTowerStat.towerId,
                                userDataTowerStat.towerLevel + 1);
                            if (userDataTowerStat.towerLevel >= 3 && neededQuantity <= currentStar &&
                                starId == MoneyType.GoldStar && neededQuantity > 0)
                            {
                                canUpgradeCases++;
                            }
                            else if (userDataTowerStat.towerLevel < 3 && neededQuantity <= currentStar &&
                                     starId == MoneyType.SliverStar && neededQuantity > 0)
                            {
                                canUpgradeCases++;
                            }
                        }
                        else
                        {
                            countTowerMaxLevel++;
                        }
                    }

                    //Case all the tower upgraded MAX level
                    if (countTowerMaxLevel == NUMBER_OF_TOWER)
                    {
                        return false;
                    }

                    if (canUpgradeCases > 0)
                    {
                        return true;
                    }
                }

                //case user data do not have tower id
                foreach (var unit in upgradeData.gradientUnits)
                {
                    if (checkedTowerIds.Contains(unit.towerId)) continue;

                    var neededQuantity =
                        upgradeData.GetStarQuantity(unit.currencyType, unit.towerId, unit.levels[1]);
                    if (neededQuantity <= currentStar && starId == MoneyType.SliverStar)
                    {
                        canUpgradeCases++;
                    }
                }

                if (canUpgradeCases > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
