using UnityEngine;
#if TRACKING_FIREBASE
using System.Threading.Tasks;
using EW2;
using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;
using Firebase.Crashlytics;
using TigerForge;
#endif
using System;
using System.Collections.Generic;
using Firebase.Messaging;

namespace Zitga.TrackingFirebase
{
    public class FirebaseProperty
    {
        public const string PlayerId = "player_id";
        public const string OnlineTime = "online_time";
        public const string Vip = "vip";
        public const string MaxNormalStage = "max_normal_stage";
        public const string IapCount = "iap_count";
        public const string IapRevenue = "iap_revenue";
        public const string RemainingCrystal = "remaining_crystal";
        public const string RemainingGem = "remaining_gem";
        public const string RemainingStamina = "remaining_stamina";
        public const string RemainingStar1 = "remaining_star_1";
        public const string RemainingStar2 = "remaining_star_2";
    }

    public class FirebaseEvent
    {
        public const string StageStart = "stage_start";
        public const string StageEnd = "stage_end";
        public const string StageTower = "stage_tower";
        public const string StageHero = "stage_hero";
        public const string StagePowerUp = "stage_power_up";
        public const string StageTowerSell = "stage_tower_sell";
        public const string EarlyWave = "early_wave";
        public const string ResourceUpdate = "resource_update";
        public const string Tutorial = "tutorial";
        public const string HeroStats = "hero_stats";
        public const string SpellLevelUp = "spell_level_up";
        public const string WaveEnd = "wave_end";
        public const string StageSpell = "stage_spell";
        public const string ButtonClick = "button_click";
        public const string TapToStart = "tap_to_start";
        public const string PurchaseClick = "purchase_click";
        public const string AdClick = "ads_click";
        public const string AdReward = "ads_reward";
        public const string DefenseStart = "defend_mode_start";
        public const string DefenseEnd = "defend_mode_end";
        public const string BuffEarn = "buff_earn";
        public const string TournamentStart = "tournament_start";
        public const string TournamentEnd = "tournament_end";
    }

    public class FirebaseParamNames
    {
        public const string TypeAction = "type";
        public const string ItemCategory = "item_category";
        public const string ItemBrand = "item_brand";
        public const string ItemId = "item_id";
        public const string ItemName = "item_name";
        public const string ItemValue = "value";
        public const string ItemRemaining = "remaining";
        public const string ItemSource = "source";
        public const string ItemSourceId = "source_id";
        public const string SourceUse = "resource_use";
        public const string SpellId = "spell_id";
        public const string SpellLevel = "spell_level";
        public const string BattleId = "battle_id";
        public const string WaveNumber = "wave_number";
        public const string DefensivePointId = "defensive_point_id";
        public const string HeroId = "hero_id_{0}";
        public const string RemainingGold = "remaining_gold";
        public const string BuffId = "buff_id";
        public const string BuffNumber = "buff_number";
        public const string TournamentBandSpellId = "tournament_band_spell_id";
        public const string TournamentNerfHeroId = "tournament_nerf_hero_id";
        public const string TournamentBandTowerId = "tournament_band_tower_id";
        public const string TournamentBuffStatId = "tournament_buff_stat_id";
        public const string TournamentNerfStatId = "tournament_nerf_stat_id";
        public const string TournamentScore = "scored";
        public const string TournamentMapId = "tournament_map_id";
        public const string TournamentWaveSurvived = "wave_survived";
        public const string TournamentDuration = "tournament_duration";
        public const string TournamentRank = "rank";
        public const string TowerBuilt = "number_of_tower_{0}_built";
    }

    public class FirebaseUtils
    {
#if TRACKING_FIREBASE
#endif
        public bool Initialized { get; private set; }

        private bool _isDebugMode;

        private string _firebaseToken;

        // When the app starts, check to make sure that we have
        // the required dependencies to use Firebase, and if not,
        // add them if possible.
        public FirebaseUtils(bool isDebugMode)
        {
#if TRACKING_FIREBASE
            Initialized = false;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    // Set default session duration values.
                    FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

                    Initialized = true;

                    this._isDebugMode = isDebugMode;

                    // Init Crashlytics
                    Application.RequestAdvertisingIdentifierAsync(
                        (string advertisingId, bool trackingEnabled, string error) => {
                            //Log.Info("advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
                            if (trackingEnabled)
                            {
                                Crashlytics.SetCustomKey("advertising_id", advertisingId);
                            }
                        }
                    );

                    // Init Remote config
                    InitializeRemoteConfig();
                    GetFirebaseToken();

                    FirebaseLogic.Instance.SetPlayerId();

                    FirebaseLogic.Instance.TrackingOnlineTime();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
#endif
        }

        public void SetUserId(string userId)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning("[Firebase] USER_ID : " + userId);
                return;
            }
#if TRACKING_FIREBASE
            FirebaseAnalytics.SetUserId(userId);
#endif
        }

        public void SetUserProperty(string name, string property)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning("[Firebase] PROPERTY : NAME " + name + " VALUE:" + property);
                return;
            }
#if TRACKING_FIREBASE
            if (Initialized)
            {
                FirebaseAnalytics.SetUserProperty(name.ToLower(), property.ToLower());
            }
            else
            {
            }
#endif
        }

        public void SetUserEvent(string eventName, string categoryValue, string nameValue)
        {
#if TRACKING_FIREBASE
            if (this._isDebugMode)
            {
                Debug.LogWarning($"[Firebase] eventName[{eventName}] category[{categoryValue}] nameValue[{nameValue}]");
                return;
            }

            var category = new Parameter("category", categoryValue);
            var name = new Parameter("name", nameValue);

            SetUserEvent(eventName, category, name);
#endif
        }

        public void SetUserEvent(string eventName, params Parameter[] args)
        {
#if TRACKING_FIREBASE
            if (this._isDebugMode)
            {
                string content = "[Firebase]";
                foreach (var parameter in args)
                {
                    content += $" [{parameter}]";
                }

                return;
            }

            FirebaseAnalytics.LogEvent(eventName, args);
#endif
        }

        #region MONEY

        public void EarnResource(string itemCategory, string itemBranch, string itemId, string itemName, float value,
            float valueRemaining, string source, string sourceId = "0")
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] EARN_RESOURCE : {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7}", itemCategory,
                    itemBranch, itemId, itemName, value, valueRemaining, source, sourceId));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.TypeAction, "earn"));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemCategory, itemCategory));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemBrand, itemBranch));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemId, itemId));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemName, itemName));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemValue, value));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemRemaining, valueRemaining));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSource, source.ToLower()));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSourceId, sourceId.ToLower()));
            FirebaseAnalytics.LogEvent(FirebaseEvent.ResourceUpdate, fParamters.ToArray());
#endif
        }

        /// <summary>
        /// Required Mothod
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <param name="itemBranch"></param>
        /// <param name="itemId"></param>
        /// <param name="value"></param>
        /// <param name="source"></param>
        public void SpendResource(string itemCategory, string itemBranch, string itemId, string itemName, float value,
            float valueRemaining, string source, string sourceId = "0")
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] SPEND_RESOURCE : {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7}", itemCategory,
                    itemBranch, itemId, itemName, value, valueRemaining, source, sourceId));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.TypeAction, "spend"));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemCategory, itemCategory));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemBrand, itemBranch));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemId, itemId));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemName, itemName));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemValue, value));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemRemaining, valueRemaining));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSource, source.ToLower()));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSourceId, sourceId.ToLower()));
            FirebaseAnalytics.LogEvent(FirebaseEvent.ResourceUpdate, fParamters.ToArray());
#endif
        }

        /// <summary>
        /// Required Mothod
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <param name="itemBranch"></param>
        /// <param name="itemId"></param>
        /// <param name="value"></param>
        /// <param name="source"></param>
        public void BuyResource(string itemCategory, string itemBranch, string itemId, string itemName, float value,
            float valueRemaining, string source, string sourceId = "0")
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] SPEND_RESOURCE : {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7}", itemCategory,
                    itemBranch, itemId, itemName, value, valueRemaining, source, sourceId));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.TypeAction, "buy"));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemCategory, itemCategory));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemBrand, itemBranch));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemId, itemId));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemName, itemName));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemValue, value.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemRemaining, valueRemaining.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSource, source.ToLower()));
            fParamters.Add(new Parameter(FirebaseParamNames.ItemSourceId, sourceId.ToLower()));
            FirebaseAnalytics.LogEvent(FirebaseEvent.ResourceUpdate, fParamters.ToArray());
#endif
        }

        public void UpgradeSpell(string resourceUse, string spellId, string spellLevel)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format("[Firebase] SPELL_LEVEL_UP : {0} - {1} - {2}", resourceUse, spellId,
                    spellLevel));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.SourceUse, resourceUse));
            fParamters.Add(new Parameter(FirebaseParamNames.SpellId, spellId));
            fParamters.Add(new Parameter(FirebaseParamNames.SpellLevel, spellLevel));
            FirebaseAnalytics.LogEvent(FirebaseEvent.SpellLevelUp, fParamters.ToArray());
#endif
        }

        public void HeroStatsChange(string heroId, string level, string awaken, List<string> skills, string spell_id,
            string spell_level, List<string> runes)
        {
            if (this._isDebugMode)
            {
                // Debug.LogWarning(string.Format(
                //     "[Firebase] HERO_STAT_CHANGE : {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14}",
                //     heroId,
                //     level, awaken, skills[0], skills[1], skills[2], skills[3], spell_id, spell_level, runes[0],
                //     runes[1], runes[2], runes[3], runes[4], runes[5]));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter("hero_id", heroId));
            fParamters.Add(new Parameter("level", level));
            fParamters.Add(new Parameter("awaken", awaken));
            for (int i = 0; i < skills.Count; i++)
            {
                fParamters.Add(new Parameter($"skill_{i + 1}", skills[i]));
            }

            fParamters.Add(new Parameter("spell_id", spell_id));
            fParamters.Add(new Parameter("spell_level", spell_level));
            for (int i = 0; i < runes.Count; i++)
            {
                fParamters.Add(new Parameter($"rune_{i + 1}", runes[i]));
            }

            FirebaseAnalytics.LogEvent(FirebaseEvent.HeroStats, fParamters.ToArray());
#endif
        }

        public void WaveEnd(int stageId, int waveNumber, int lifePoint, int spellNumber)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format("[Firebase] SPELL_LEVEL_UP : {0} - {1} - {2} - {3}", stageId, waveNumber,
                    lifePoint, spellNumber));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter("stage", stageId));
            fParamters.Add(new Parameter("wave_number", waveNumber));
            fParamters.Add(new Parameter("life_point", lifePoint));
            fParamters.Add(new Parameter("spell_number", spellNumber));
            FirebaseAnalytics.LogEvent(FirebaseEvent.WaveEnd, fParamters.ToArray());
#endif
        }

        public void StageSpell(int stageId, int win, int spell, int spell_level, int number, int hero_id)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format("[Firebase] STAGE_SPELL : {0} - {1} - {2} - {3} - {4} - {5}", stageId,
                    win, spell, spell_level, number, hero_id));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter("stage", stageId));
            fParamters.Add(new Parameter("win", win));
            fParamters.Add(new Parameter("spell", spell));
            fParamters.Add(new Parameter("spell_level", spell_level));
            fParamters.Add(new Parameter("number", number));
            fParamters.Add(new Parameter("hero_id", hero_id));
            FirebaseAnalytics.LogEvent(FirebaseEvent.StageSpell, fParamters.ToArray());
#endif
        }

        #endregion

        #region REMOTE CONFIG

#if TRACKING_FIREBASE

        void InitializeRemoteConfig()
        {
            // [START set_defaults]
            var defaults = GetRemoteConfigDefault();

            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                .ContinueWithOnMainThread(SetDefaultComplete);
        }

        void SetDefaultComplete(Task task)
        {
            // [END set_defaults]
            Debug.Log("RemoteConfig configured and ready!");

            FetchDataAsync();
        }

        // [START fetch_async]
        // Start a fetch request.
        // FetchAsync only fetches new data if the current data is older than the provided
        // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
        // By default the timespan is 12 hours, and for production apps, this is a good
        // number. For this example though, it's set to a timespan of zero, so that
        // changes in the console will always show up immediately.
        public Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");

            var cacheTime = TimeSpan.FromHours(2); // when in production --> cache time set to 2 hours
            Task fetchTask =
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                    cacheTime);

            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }
        //[END fetch_async]

        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                        .ContinueWithOnMainThread(task => {
                            Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                info.FetchTime));
                            Debug.Log(
                                $"Getcampaignid {Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigParam.CampaignLevelDesignId).LongValue}");
                            Debug.Log(
                                $"IpaId {Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigParam.IapConfigId).LongValue}");
                        });

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }

        private Dictionary<string, object> GetRemoteConfigDefault()
        {
            var defaults = new Dictionary<string, object> {
                { RemoteConfigParam.CampaignLevelDesignId, 0 }, { RemoteConfigParam.IapConfigId, 0 }
            };

            return defaults;
        }

        private void GetFirebaseToken()
        {
            FirebaseMessaging.GetTokenAsync().ContinueWith(
                task => {
                    if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
                    {
                        Debug.Log($"FIREBASE ID TOKEN: {task.Result}");
                        _firebaseToken = task.Result;
                    }
                });
        }

        #region API

        public string GetFirebaseTokenIdString()
        {
            return _firebaseToken;
        }

        public int GetCampaignLevelDesignId()
        {
            var campaignSpawnId = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(
                RemoteConfigParam.CampaignLevelDesignId).LongValue;
            Debug.LogWarning($"campaignSpawnId {campaignSpawnId}");
            return campaignSpawnId;
        }

        public int GetIAPConfig()
        {
            var iapConfigId = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(
                RemoteConfigParam.IapConfigId).LongValue;
            Debug.LogWarning($"iapConfigId {iapConfigId}");
            return iapConfigId;
        }

        #endregion

        #region FakeData

        public void FakeCampaignLevelDesign(int id)
        {
            var defaults = GetRemoteConfigDefault();
            defaults[RemoteConfigParam.CampaignLevelDesignId] = id;
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        }

        #endregion

#endif

        #endregion

        #region HERO DEFENSE

        public void HeroDefenseStart(long battleId, int dfpId, List<int> listHeroIds)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] Hero Defense Start : {0} - {1}", battleId, dfpId));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.BattleId, battleId.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.DefensivePointId, dfpId.ToString()));
            for (int i = 0; i < listHeroIds.Count; i++)
            {
                fParamters.Add(new Parameter(string.Format(FirebaseParamNames.HeroId, i + 1), listHeroIds[i]));
            }

            FirebaseAnalytics.LogEvent(FirebaseEvent.DefenseStart, fParamters.ToArray());
#endif
        }

        public void HeroDefenseEnd(long battleId, int waveCount, long remainingGold)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] Hero Defense End : {0} - {1} - {2} ", battleId, waveCount, remainingGold));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.BattleId, battleId.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.WaveNumber, waveCount.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.RemainingGold, remainingGold.ToString()));

            FirebaseAnalytics.LogEvent(FirebaseEvent.DefenseEnd, fParamters.ToArray());
#endif
        }

        public void HeroDefenseBuffErn(long battleId, int waveCount, string buffId, int buffNumber)
        {
            if (this._isDebugMode)
            {
                Debug.LogWarning(string.Format(
                    "[Firebase] Hero Defense End : {0} - {1} - {2} - {3}", battleId, waveCount, buffId, buffNumber));
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.BattleId, battleId.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.WaveNumber, waveCount.ToString()));
            fParamters.Add(new Parameter(FirebaseParamNames.BuffId, buffId));
            fParamters.Add(new Parameter(FirebaseParamNames.BuffNumber, buffNumber.ToString()));

            FirebaseAnalytics.LogEvent(FirebaseEvent.BuffEarn, fParamters.ToArray());
#endif
        }

        #endregion

        #region TOURNAMENT

        public void PlayATournamentGame(List<int> listHeroIds, int bandSpellId, int nerfHeroId, int bandTowerId,
            int statBuffId, int nerfStatId)
        {
            if (this._isDebugMode)
            {
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentBandSpellId, bandSpellId));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentNerfHeroId, nerfHeroId));
            for (int i = 0; i < listHeroIds.Count; i++)
            {
                fParamters.Add(new Parameter(string.Format(FirebaseParamNames.HeroId, i + 1), listHeroIds[i]));
            }

            fParamters.Add(new Parameter(FirebaseParamNames.TournamentBandTowerId, bandTowerId));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentNerfStatId, nerfStatId));

            fParamters.Add(new Parameter(FirebaseParamNames.TournamentBuffStatId, statBuffId));
            FirebaseAnalytics.LogEvent(FirebaseEvent.TournamentStart, fParamters.ToArray());
#endif
        }

        public void ResultATournamentGame(int scored, int waveSurvived, int tournamentMapId, int previousRank,
            int remainingGold, float playDuration, Dictionary<int, int> builtTowers)
        {
            if (this._isDebugMode)
            {
                return;
            }
#if TRACKING_FIREBASE
            List<Parameter> fParamters = new List<Parameter>();
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentScore, scored));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentWaveSurvived, waveSurvived));
            fParamters.Add(new Parameter(FirebaseParamNames.RemainingGold, remainingGold));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentMapId, tournamentMapId));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentRank, previousRank));
            fParamters.Add(new Parameter(FirebaseParamNames.TournamentDuration, playDuration));
            //towerId, count
            foreach (KeyValuePair<int, int> builtTower in builtTowers)
            {
                fParamters.Add(new Parameter(string.Format(FirebaseParamNames.TowerBuilt, builtTower.Key),
                    builtTower.Value));
            }

            FirebaseAnalytics.LogEvent(FirebaseEvent.TournamentEnd, fParamters.ToArray());
#endif
        }

        #endregion
    }
}