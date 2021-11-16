using System;

namespace EW2
{
    public static class GamePlayEvent
    {
        public static readonly string StartInitAppsflyer = "OnStartInitAppsflyer";

        // Global event 
        public static string OnMoneyChange(ResourceType type, int moneyType)
        {
            return $"OnMoneyChange_{type}_{moneyType}";
        }

        public static string OnAddMoney(ResourceType type, int moneyType)
        {
            return $"OnAddMoney{type}_{moneyType}";
        }

        public static string OnSubMoney(ResourceType type, int moneyType)
        {
            return $"OnSubMoney{type}_{moneyType}";
        }

        // noti
        public static readonly string ShortNoti = "ShortNoti";
        public static readonly string CloseLoading = "CloseLoading";
        public const string DAILY_CHECKIN = "DailyCheckin";
        public const string VIEW_AD_VIDEO = "ViewAdVideo";
        public const string FIRST_PURCHASE_CLAIMED = "FirstPurchase";
        public const string CLICK_COMMUNITY = "ClickCommunity";

        //UI Game Play
        public static readonly string UIStateChanged = "UIStateChanged";

        public static readonly string SelectingHero = "SelectingHero";

        public static readonly string OnTargetDone = "OnTargetDone";

        public static readonly string OnRallyDone = "OnRallyDone";
        public static readonly string OnSpellSelectTarget = "OnRallyDone";

        // Game Play
        public static readonly string OnConfirmCallWave = "OnConfirmCallWave";

        public static readonly string OnStartGame = "OnStartGame";

        public static readonly string OnCallWaveFromShip = "OnCallWaveFromShip";

        public static readonly string OnEnemyDie = "OnEnemyDie";

        public static readonly string OnEndSpawn = "OnEndSpawn";

        public static readonly string OnEndGame = "OnEndGame";


        public static Action onCallWaveSpecial;

        public static readonly string OnChangeNameSuccess = "ChangeNameSuccess";

        public static readonly string OnChangeAvatarSuccess = "ChangeAvatarSuccess";

        public static readonly string OnChangeLanggueSuccess = "ChangeLanggueSuccess";

        public static readonly string onHeroDeSpawn = "OnHeroDeSpawn";

        public static readonly string onHeroRevive = "OnHeroRevive";

        public static readonly string OnHeroSelectChange = "OnHeroSelectChange";

        public static readonly string OnHeroUnlocked = "OnHeroUnlocked";

        public static readonly string OnHeroLevelUp = "OnHeroLevelUp";

        public static readonly string OnUpgradeSkillHero = "OnUpgradeSkillHero";

        public static readonly string OnResetAllSkill = "OnResetAllSkill";

        public static readonly string OnRefreshHeroRoom = "OnRefreshHeroRoom";

        public static readonly string OnRefreshSpell = "OnRefreshSpell";

        public static readonly string OnRefreshRune = "OnRefreshRune";

        public static readonly string OnEnhanceRune = "OnEnhanceRune";

        public static readonly string OnEquipRune = "OnEquipRune";

        //in game play
        public const string ON_SPELL_INIT_READY = "OnSpellInitReady";


        public static readonly string OnUpdateBadge = "OnUpdateBadge";

        public static readonly string OnUpdateRangeAttack = "OnUpdateRangeAttack";

        public static readonly string OnLoginSuccess = "LoginSuccess";

        public static readonly string OnReInitIap = "OnReInitIap";

        public static readonly string OnUpdateSaleBundle = "OnUpdateSaleBundle";

        public static readonly string OnFocusTabShop = "OnFocusTabShop";

        public static readonly string OnRefreshSpellGacha = "OnRefreshSpellGacha";

        public static readonly string OnRefreshRuneGacha = "OnRefreshRuneGacha";

        public static readonly string OnConvertFragmentSpell = "OnConvertFragmentSpell";

        public static readonly string OnLoadDataSuccess = "OnLoadDataSuccess";

        #region Quest Event

        public static readonly string OnCompleteDailyQuest = "OnCompleteDailyQuest";

        public static readonly string OnCompleteAllDailyQuest = "OnCompleteAllDailyQuest";

        public static readonly string OnClaimCheckinDaily = "OnClaimCheckinDaily";

        public static readonly string OnSaveData = "OnSaveData";

        public static readonly string OnSpellUpgrade = "OnSpellUpgrade";

        public static readonly string OnSummon = "OnSummon";

        public static readonly string OnUseSpell = "OnUseSpell";

        public static readonly string OnGameStart = "OnGameStart";

        public static readonly string OnPlayCampaign = "OnPlayCampaign";

        public static readonly string OnHeroUseActiveSkill = "OnHeroUseActiveSkill";

        public static readonly string OnEnemyKill = "OnEnemyKill";

        public static readonly string OnEarnStarCampaign = "OnEarnStarCampaign";

        public static readonly string OnSpentStarCampaign = "OnSpentStarCampaign";

        public static readonly string OnGachaSpell = "OnGachaSpell";

        public static readonly string OnEarnSpell = "OnEarnSpell";

        public static readonly string OnEquipSpell = "OnEquipSpell";

        public static readonly string OnSpendResource = "OnSpendResource";

        public static readonly string OnJointFanpage = "OnJointFanpage";

        public static readonly string OnGachaRune = "OnGachaRune";

        public static readonly string OnMapComplete = "OnMapComplete";

        public static readonly string OnWatchVideo = "OnWatchVideo";

        public static readonly string OnIAP = "OnIAP";

        #endregion

        #region GloryRoad

        public const string OnRepaintGloryRoad = "OnRepaintGloryRoad";

        #endregion

        public const string OnApplicationQuit = "OnApplicationQuit";

        public const string OnRefreshFormationDefense = "OnRefreshFormationDefense";

        public const string OnRefreshDefensePoint = "OnRefreshDefensePoint";

        public const string OnHealthPointDFPUpdate = "OnHealthPointDFPUpdate";

        public const string OnNewSeasonHeroDefense = "OnNewSeasonHeroDefense";

        public const string OnChangeHeroBuffTournament = "OnChangeHeroBuffTournament";

        public const string OnClaimRewardTournament = "OnClaimRewardTournament";
    }
}