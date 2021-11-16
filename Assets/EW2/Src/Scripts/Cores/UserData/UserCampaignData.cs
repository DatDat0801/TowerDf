using System.Collections.Generic;
using System.Linq;

namespace EW2
{
    public class UserCampaignData
    {
        public Dictionary<int, int> CampaignDict { get; }
        public HashSet<int> PlayedCampaign { get; }

        public List<int> listEnemyUnlocked = new List<int>();

        /// <summary>
        /// show the UI every world after nightmare unlocked
        /// </summary>
        public List<bool> nightmareNoticed;

        public bool isCanShowStarterPack;

        public UserCampaignData()
        {
            CampaignDict = new Dictionary<int, int>();
            PlayedCampaign = new HashSet<int>();
            nightmareNoticed = new List<bool>();
        }

        public void SetStar(int campaignId, int star)
        {
            if (CampaignDict.ContainsKey(campaignId))
            {
                if (star > CampaignDict[campaignId])
                {
                    CampaignDict[campaignId] = star;
                }
            }
            else
            {
                if (star < 0)
                {
                    CampaignDict.Remove(campaignId);
                }
                else
                {
                    CampaignDict[campaignId] = star;
                }
            }
        }

        public (int, int) GetMaxStar(int worldId, int stageId)
        {
            var star = GetStar(worldId, stageId, 1);

            if (star > 0)
            {
                return (1, star);
            }

            return (0, GetStar(worldId, stageId, 0));
        }

        public int GetStar(int worldId, int stageId)
        {
            return GetStar(worldId, stageId, 0) + GetStar(worldId, stageId, 1);
        }

        public int GetStar(int worldId, int stageId, int modeId)
        {
            return GetStar(MapCampaignInfo.GetCampaignId(worldId, stageId, modeId));
        }

        public int GetStar(int campaignId)
        {
            if (CampaignDict.ContainsKey(campaignId))
                return CampaignDict[campaignId];
            return 0;
        }

        public int HighestStageUnlocked(int modeId)
        {
            HashSet<int> stages = new HashSet<int>();
            for (int i = 0; i < CampaignDict.Count; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, modeId);
                if (CampaignDict.ContainsKey(campaignId))
                    if (CampaignDict[campaignId] > 0)
                        stages.Add(i);
            }

            return stages.Count;
        }

        /// <summary>
        /// count win levels
        /// </summary>
        /// <returns></returns>
        public int HighestPassLevel()
        {
            HashSet<int> stages = new HashSet<int>();
            for (int i = 0; i < CampaignDict.Count; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, 0);
                if (CampaignDict.ContainsKey(campaignId))
                {
                    if (CampaignDict[campaignId] >= 1)
                        stages.Add(i);
                }
            }

            return stages.Count;
        }

        /// <summary>
        /// Get the highest stage that user play to the end of the map 
        /// </summary>
        /// <returns></returns>
        public int HighestResultLevel()
        {
            HashSet<int> stages = new HashSet<int>();
            for (int i = 0; i < CampaignDict.Count; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, 0);
                if (CampaignDict.ContainsKey(campaignId))
                {
                    if (CampaignDict[campaignId] >= 0)
                        stages.Add(i);
                }
            }

            return stages.Count;
        }
        public int GetTotalStars(int modeId)
        {
            int result = 0;
            HashSet<int> stages = new HashSet<int>();
            for (int i = 0; i < CampaignDict.Count; i++)
            {
                var campaignid = MapCampaignInfo.GetCampaignId(0, i, modeId);
                if (CampaignDict.ContainsKey(campaignid))
                {
                    stages.Add(campaignid);
                }
            }

            foreach (var stage in stages)
            {
                result += GetStar(stage);
            }


            return result;
        }
        public int GetCollectedStars(int starId)
        {
            return (int)UserData.Instance.GetMoney(starId);

        }

        public bool IsUnlockedStage(int worldId, int stageId)
        {
            int mapCondition = GameContainer.Instance.Get<MapDataBase>().GetUnlockCampaign(worldId, stageId);

            if (mapCondition < 0)
                return true;

            return GetStar(mapCondition) > 0;
        }

        public bool IsUnlockedHardStage(int worldId, int stageId)
        {
            var star = GetStar(worldId, stageId, 0);

            return star == 3;
        }

        public bool IsCurrentStage(int worldId, int stageId, int modeId)
        {
            return IsUnlockedStage(worldId, stageId) && GetStar(worldId, stageId, modeId) == 0;
        }

        public void AddEnemyUnlocked(int enemyId)
        {
            if (!listEnemyUnlocked.Contains(enemyId))
            {
                listEnemyUnlocked.Add(enemyId);
            }
        }

        public bool CheckEnemyUnlocked(int enemyId)
        {
            if (listEnemyUnlocked.Contains(enemyId)) return true;

            return false;
        }


        public int GetHighestPlayedStage()
        {
            var mapNotInPlayedCampaign = CampaignDict.Select(x => x.Value).Except(PlayedCampaign);

            //add current data if exist
            if (mapNotInPlayedCampaign.Any())
            {
                foreach (var p in mapNotInPlayedCampaign)
                {
                    PlayedCampaign.Add(p);
                }
            }

            return PlayedCampaign.Count - 1;
        }

        public void SetAsPlayed(int stage)
        {
            if (!PlayedCampaign.Contains(stage))
                PlayedCampaign.Add(stage);
        }

        public int GetMapIdHighest()
        {
            var id = 0;
            foreach (var idMap in CampaignDict.Keys)
            {
                id = idMap;
            }

            return id;
        }

        public int GetStageCurrent()
        {
            var mapId = 0;
            var haveResult = false;
            var numberMap = GameContainer.Instance.Get<MapDataBase>().GetAllReward().rewardCampaigns.Count / 2;
            for (int i = 0; i < numberMap; i++)
            {
                mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Normal);
                if (GetStar(mapId) <= 0)
                {
                    haveResult = true;
                    break;
                }
            }

            if (!haveResult)
            {
                for (int i = 0; i < numberMap; i++)
                {
                    mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Normal);
                    var star = GetStar(mapId);
                    if (star > 0 && star < 3)
                    {
                        haveResult = true;
                        break;
                    }
                }
            }

            if (!haveResult)
                mapId = MapCampaignInfo.GetCampaignId(0, numberMap - 1, (int)ModeCampaign.Normal);

            return mapId;
        }

        public int GetStageNightmareCurrent()
        {
            var mapId = 0;
            var haveResult = false;
            var numberMap = GameContainer.Instance.Get<MapDataBase>().GetAllReward().rewardCampaigns.Count / 2;
            for (int i = 0; i < numberMap; i++)
            {
                mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Normal);
                if (GetStar(mapId) >= 3)
                {
                    mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Nightmare);
                    if (GetStar(mapId) <= 0)
                    {
                        haveResult = true;
                        break;
                    }
                }
            }

            if (!haveResult)
            {
                for (int i = 0; i < numberMap; i++)
                {
                    mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Nightmare);
                    var star = GetStar(mapId);
                    if (star > 0 && star < 3)
                    {
                        haveResult = true;
                        break;
                    }
                }
            }

            if (!haveResult)
            {
                for (int i = 0; i < numberMap; i++)
                {
                    mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Normal);
                    if (GetStar(mapId) >= 3)
                    {
                        mapId = MapCampaignInfo.GetCampaignId(0, i, (int)ModeCampaign.Nightmare);
                        if (GetStar(mapId) > 0 && GetStar(mapId) < 3)
                        {
                            haveResult = true;
                            break;
                        }
                    }
                }
            }

            if (!haveResult)
                mapId = GetStageCurrent();

            return mapId;
        }

        public void AddNewWorldNightmareUnlocked()
        {
            //world id is the same index of the
            nightmareNoticed.Add(true);
        }

        public bool IsShowUnlockNightmareNotice(int worldId)
        {
            if (nightmareNoticed.Count <= 0) return false;
            return nightmareNoticed[worldId];
        }
    }
}
