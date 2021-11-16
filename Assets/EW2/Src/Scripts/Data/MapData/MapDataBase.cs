using UnityEngine;
using Zitga.ContextSystem;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class MapDataBase
    {
        private const string ROOT = "CSV/MapCampaigns";
        private const string ROOT_TEST = "CSV/MapCampaigns_";

        private readonly ServiceContainer _container;

        public MapDataBase()
        {
            this._container = new ServiceContainer();
        }

        public T GetMap<T>(int worldId, int mapId, int mode) where T : ScriptableObject
        {
            string key = $"{ROOT}/Map_{worldId}_{mapId}_{mode}";

#if TRACKING_FIREBASE
            var idDataRemoteConfig = FirebaseLogic.Instance.GetCampaignLevelDesignDataId();
            Debug.Log($"IdMapLevelDataRemoteConfig {idDataRemoteConfig}");
            if (idDataRemoteConfig > 0)
                key = $"{ROOT_TEST}{idDataRemoteConfig}/Map_{worldId}_{mapId}_{mode}";
#endif

            var obj = this._container.Resolve<T>(key);
            if (obj == null)
            {
                obj = Resources.Load<T>(key);

                this._container.Register(key, obj);
            }

            return obj;
        }

        public T GetMapGoldDrop<T>(int worldId, int mode) where T : ScriptableObject
        {
            var obj = this._container.Resolve<T>($"Map_Gold_Drop_{worldId}_{mode}");
            if (obj == null)
            {
                obj = LoadGoldDrop<T>(worldId, mode);

                this._container.Register($"Map_Gold_Drop_{worldId}_{mode}", obj);
            }

            return obj;
        }

        private T LoadGoldDrop<T>(int worldId, int mode) where T : ScriptableObject
        {
            string key = $"{ROOT}/Map_Gold_Drop_{worldId}_{mode}";

#if TRACKING_FIREBASE
            var idDataRemoteConfig = FirebaseLogic.Instance.GetCampaignLevelDesignDataId();
            Debug.Log($"IdMapLevelDataRemoteConfig {idDataRemoteConfig}");
            if (idDataRemoteConfig > 0)
                key = $"{ROOT_TEST}{idDataRemoteConfig}/Map_Gold_Drop_{worldId}_{mode}";
#endif

            return Resources.Load<T>(key);
        }

        public AllRewardCampaignInfo GetAllReward()
        {
            var obj = this._container.Resolve<AllRewardCampaignInfo>();
            if (obj == null)
            {
                obj = LoadAllReward();

                this._container.Register(obj);
            }

            return obj;
        }

        private AllRewardCampaignInfo LoadAllReward()
        {
            return Resources.Load<AllRewardCampaignInfo>($"{ROOT}/Rewards");
        }

        public int GetUnlockCampaign(int worldId, int mapId)
        {
            return GetUnlockCampaignData().GetUnlockMap(MapCampaignInfo.GetCampaignId(worldId, mapId, 0));
        }

        public int GetNextCampaign(int worldId, int mapId)
        {
            return GetUnlockCampaignData().GetNextMap(MapCampaignInfo.GetCampaignId(worldId, mapId, 0));
        }

        public TrialHeroData GetTrialHeroData()
        {
            var obj = this._container.Resolve<TrialHeroData>("TrialHeroData");
            if (obj == null)
            {
                obj = Resources.Load<TrialHeroData>($"{ROOT}/TrialHeroData");

                this._container.Register("TrialHeroData", obj);
            }

            return obj;
        }

        protected UnlockCampaignData GetUnlockCampaignData()
        {
            var obj = this._container.Resolve<UnlockCampaignData>();
            if (obj == null)
            {
                obj = SetUnlockCampaign();

                this._container.Register(obj);
            }

            return obj;
        }

        private UnlockCampaignData SetUnlockCampaign()
        {
            var allRewards = GetAllReward();

            return new UnlockCampaignData(allRewards);
        }
    }
}