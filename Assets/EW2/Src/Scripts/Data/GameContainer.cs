using EW2.DailyCheckin;
using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class GameContainer
    {
        private ServiceContainer container;

        private static readonly GameContainer Ins = new GameContainer();

        public static GameContainer Instance => Ins;

        private GameContainer()
        {
            container = new ServiceContainer();
        }

        private T GetResource<T>(string path) where T : ScriptableObject
        {
            var obj = container.Resolve<T>();
            if (obj == null)
            {
                obj = LoadResource<T>(path);
                container.Register(obj);
            }

            return obj;
        }

        private T LoadResource<T>(string path) where T : ScriptableObject
        {
            return Resources.Load<T>(path);
        }

        public T Get<T>() where T : new()
        {
            var obj = container.Resolve<T>();
            if (obj == null)
            {
                obj = new T();
                container.Register(obj);
            }

            return obj;
        }

        #region Tower

        public TowerData GetTowerData(int towerId)
        {
            switch (towerId)
            {
                case 2001:
                    return Get<UnitDataBase>().Get<TowerData2001>();
                case 2002:
                    return Get<UnitDataBase>().Get<TowerData2002>();
                case 2003:
                    return Get<UnitDataBase>().Get<TowerData2003>();
                case 2004:
                    return Get<UnitDataBase>().Get<TowerData2004>();
                default:
                    return null;
            }
        }

        public TowerRaiseCost GetTowerCost(int towerId)
        {
            return Get<UnitDataBase>().Get<TowerCost>().raiseCosts[towerId];
        }

        public TowerUnlock[] GetTowerUnlocks()
        {
            return GetResource<TowerUnlockData>("CSV/Units/TowerUnlockData").unlocks;
        }

        public TowerUpgradeData GetTowerUpgradeData()
        {
            return GetResource<TowerUpgradeData>("CSV/UpgradeTowerSystem/TowerUpgradeData");
        }

        #endregion

        #region Map Campaign

        public MapCampaignInfo GetMapData(int campaignId)
        {
            var (worldId, mapId, mode) = MapCampaignInfo.GetWorldMapModeId(campaignId);

            return Get<MapDataBase>().GetMap<MapCampaignInfo>(worldId, mapId, mode);
        }

        public MapCampaignInfo GetMapData(int worldId, int mapId, int mode)
        {
            return Get<MapDataBase>().GetMap<MapCampaignInfo>(worldId, mapId, mode);
        }

        public WorldMapGoldDrop GetWorldMapGoldDrop(int worldId, int mode)
        {
            return Get<MapDataBase>().GetMapGoldDrop<WorldMapGoldDrop>(worldId, mode);
        }

        public int GetWorldMapSize(int worldId)
        {
            var worldMap = Get<MapDataBase>().GetMapGoldDrop<WorldMapGoldDrop>(worldId, 0);

            return worldMap.goldDropInfos.Length;
        }

        #endregion

        #region Hero

        public HeroData GetHeroData(int heroId)
        {
            switch (heroId)
            {
                case (int)HeroType.Arryn:
                    return Get<UnitDataBase>().Get<HeroData1002>();
                case (int)HeroType.Jave:
                    return Get<UnitDataBase>().Get<HeroData1001>();
                case (int)HeroType.Neetan:
                    return Get<UnitDataBase>().Get<HeroData1003>();
                case (int)HeroType.Marco:
                    return Get<UnitDataBase>().Get<HeroData1004>();
                case (int)HeroType.NeroCat:
                    return Get<UnitDataBase>().Get<HeroData1005>();

                default:
                    return null;
            }
        }

        public HeroUnlockConditionData GetHeroUnlockData()
        {
            return GetResource<HeroUnlockConditionData>("CSV/Units/HeroUnlockConditionData");
        }

        #endregion

        public Reward[] GetPreRegister()
        {
            return GetResource<PreRegisterData>("CSV/PreRegister/PreRegisterData").rewards;
        }

        public HeroCollection GetHeroCollection()
        {
            return GetResource<HeroCollection>("CSV/Units/HeroCollection");
        }

        public HeroSkillUpgradeData GetHeroSkillUpgrade()
        {
            return GetResource<HeroSkillUpgradeData>("CSV/HeroRoom/HeroSkillUpgradeData");
        }

        #region Tutorial

        public TutorialData GetTutorialData()
        {
            string fullPath = "CSV/Tutorials/tutorial_data";

            return GetResource<TutorialData>(fullPath);
        }

        #endregion

        #region Daily Checkin

        public DailyCheckinDatabase GetDailyCheckinDb()
        {
            string fullPath = "CSV/DailyCheckin/DailyCheckinDatabase";
            return GetResource<DailyCheckinDatabase>(fullPath);
        }

        #endregion

        #region AdsData

        public AdRewardDatabase GetAdRewardDatabase()
        {
            string fullPath = "CSV/RewardedAd/AdRewardDatabase";
            return GetResource<AdRewardDatabase>(fullPath);
        }

        #endregion

        #region Inventory

        public RuneDismantleDatabase GetRuneDismantleDatabase()
        {
            return GetResource<RuneDismantleDatabase>("CSV/Inventory/RuneDismantleDatabase");
        }

        #endregion

        #region Events

        public FirstPurchaseRewardDatabase GetFirstPurchaseReward()
        {
            return GetResource<FirstPurchaseRewardDatabase>(
                "CSV/GameOpsEvent/FirstPurchase/FirstPurchaseRewardDatabase");
        }

        public GloryRoadDatabase GetGloryRoadData()
        {
            return GetResource<GloryRoadDatabase>("CSV/GameOpsEvent/GloryRoad/GloryRoadDatabase");
        }

        public CommunityRewardDatabase GetCommunityReward()
        {
            return GetResource<CommunityRewardDatabase>("CSV/GameOpsEvent/CommunityRewardDatabase");
        }



        #endregion

        #region Rating

        public RatingData GetRatingDatabase()
        {
            string fullPath = "CSV/RatingData/RatingData";
            return GetResource<RatingData>(fullPath);
        }

        #endregion

        #region Map Hero Defense

        public DefensiveModeMapData GetMapDefensiveData(int mapId)
        {
            return Get<DefendModeDataBase>().GetMap(mapId);
        }

        #endregion

        #region Tournament

        public TournamentDatabase GetTournamentData()
        {
            return GetResource<TournamentDatabase>("CSV/Tournament/TournamentDatabase");
        }

        public TournamentMapData GetTournamentMapData(int tournamentMapId)
        {
            return Get<TournamentDataContainer>().GetMap<TournamentMapData>(tournamentMapId);
        }

        public TournamentGoldDropData GetTournamentGoldDropData(int tournamentMapId)
        {
            return Get<TournamentDataContainer>().GetMapGoldDrop<TournamentGoldDropData>(tournamentMapId);
        }

        #endregion
    }
}