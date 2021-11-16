namespace EW2
{
    public class SceneName
    {
        public static string Start = "StartScene";
        public static string GamePlay = "GamePlayScene";
        public static string RootScene = "RootScene";
        public static string HeroDefenseGamePlay = "HeroDefenseGamePlay";
        public static string TournamentGameplay = "GamePlayTournament";
    }

    public enum TowerType
    {
        None,
        Barrack
    }

    public enum PlayMode
    {
        None,
        Campaign,
        ReplayCampaign,
        Lobby
    }

    public enum ModeCampaign
    {
        Normal = 0,
        Nightmare = 1
    }

    public enum GameMode
    {
        CampaignMode = 0,
        DefenseMode = 1,
        TournamentMode = 2
    }

    public enum UnitType
    {
        None = -1,
        Hero,
        Tower,
        Enemy
    }

    public enum EffectOnType
    {
        None = -1,
        Hero = 0,
        GroundEnemy = 1,
        FlyEnemy = 2,
        All = 3
    }

    public enum UnitClassType
    {
        None = -1,
        Assassin,
        Warrior,
        Witch,
        Archer,
    }

    public enum BuildingId
    {
        Barrack = 2004,
        Archer = 2001,
        Mage = 2002,
        Golem = 2003,
    }

    public enum BranchType
    {
        Skill1 = 0,
        Skill2 = 1,
    }

    public enum AttackType
    {
        None = -1,
        Melee = 0,
        Range = 1,
        DOT = 2,
    }

    public enum MoveType
    {
        None = -1,
        Ground = 0,
        Fly = 1,
        All = 2,
    }

    public enum PriorityTargetType
    {
        None = -1,
        Marginal = 0,
        Random = 1,
        Close = 2,
        Fly = 3
    }

    public enum TraitEnemyType
    {
        None = -1,
        Trickster = 0,
        Warmonger = 1,
        Skirmisher = 2
    }

    public enum HeroType
    {
        None = -1,
        Jave = 1001,
        Arryn = 1002,
        Neetan = 1003,
        Marco = 1004,
        NeroCat = 1005
    }

    public enum DamageType
    {
        None = -1,
        Physical = 0,
        Magical = 1,
        True = 2
    }

    public enum EffectType : byte
    {
        None,
        Enemy,
        Hero,
        Tower,
    }

    public enum GamePlayState
    {
        Init,
        Called,
        End
    }

    public enum Location
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum UnitSize
    {
        Tiny = 0,
        Small = 1,
        Hero = 2,
        MiniBoss = 3,
        Boss = 4
    }

    public enum SkillIndex
    {
        SkillActive = 0,
        SkillPassive1 = 1,
        SkillPassive2 = 2,
        SkillPassive3 = 3
    }

    public class CampaignMode
    {
        public const int Normal = 0;
        public const int Nightmare = 1;
    }

    public enum HeroClasses
    {
        Melee = 0,
        Ranged = 1,
        Tank = 2,
        Assassin = 3,
        Aoe = 4,
        Summoner = 5,
        Control = 6,
        Buff = 7,
        Debuff = 8
    }

    public enum ShopTabId
    {
        None = -1,
        Gem = 0,
        Crystal = 1,
        DefensivePoint = 2,
        Tournament = 3
    }

    public enum GachaTabId
    {
        None = -1,
        Rune = 0,
        Spell = 1,
    }

    public enum SaleType
    {
        None = -1,
        Hot = 0,
        Best = 1
    }

    /// <summary>
    /// Auto means the spell automatically execute,
    /// Manually means it depend on user's interact 
    /// </summary>
    public enum SpellUseType
    {
        Auto = 0,
        Manually = 1
    }

    // public enum SpellCreateType
    // {
    //     /// <summary>
    //     /// the spell assign to its hero
    //     /// </summary>
    //     CreatorAsHero,
    //     /// <summary>
    //     /// the spell executes independently
    //     /// </summary>
    //     CreateByItSelf
    // }
    public enum HeroRoomTabId
    {
        None = -1,
        Skill = 0,
        Spell = 1,
        Rune = 2,
    }

    public enum TargetType
    {
        Login,
        MapCampaignNormal,
        MapCampaignNightmare,
        AnyHeroUpgrade,
        SaveLoad,
        CheckinDaily,
        SummonSpell,
        AnySpellUpgrade,
        UseAnySpell,
        AllDailyQuest,
        AnyHero,
        AnyHeroUpgradeMax,
        UseSkillAnyHero,
        ArcherKill,
        GolemKill,
        MagicKill,
        BarrackKill,
        KillBoss,
        EarnStar,
        SpendSliverStar,
        SpendGoldStar,
        GachaSpell,
        NormalSpell,
        SpellLevelMax,
        EquipSpell,
        Gem,
        Crystal,
        JoinFanpage,
        DailyQuest,
        KillAnyEnemy,
        AnyMapNormal,
        AnyMapNightmare,
        Complete3StarNormal,
        Complete3StarNightmare,
        AnySpellLevel,
        SummonRunePremium,
        SummonRune,
        AnyRune,
        AnyTower,
        WatchVideo,
        IAP,
        Hero,
        Complete3StarNormalWithHero,
        Complete3StarNightmareWithHero,
        UseSkillHero,
        UpgradeHero,
        EquipRuneForHero,
        EquipSpellForHero
    }

    public enum QuestType
    {
        Play,
        Summon,
        Upgrade,
        Save,
        Complete,
        Online,
        Use,
        Count,
        Kill,
        KillBy,
        Earn,
        Spent,
        Gacha
    }

    public enum BossId
    {
        Boss10 = 3018,
        Boss20 = 3020
    }

    public enum RuneSet : int
    {
        RuneSet2 = 2,
        RuneSet4 = 4,
        RuneSet6 = 6
    }

    public enum RuneId : int
    {
        PowerRune = 0,
        GuardRune = 1,
        EndureRune = 2,
        VitalityRune = 3,
        ViolentRune = 4,
        RageRune = 5,
        ImmortalRune = 6,
        DeathRune = 7,
        LifeRune = 8,
        WisdomRune = 9,
        ArgonyRune = 10,
        MiseryRune = 11
    }
}