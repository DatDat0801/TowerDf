using System.Collections.Generic;

namespace EW2
{
    public static class GameConfig
    {
        public const int HeroCount = 5;

        public const int IntervalRegeneration = 1;

        public const int NumberWord = 1;

        public const float TimeDelayStartWave = 21;

        public const float TimeDelayShowCallWave = 3f;

        public static readonly List<int> ListBossIds = new List<int>() {3015, 3016, 3017, 3018, 3019, 3020};

        public const int MaxNumberStarNormal = 3;

        public const int MaxNumberStarHard = 6;

        public static int MaxStamina = 30;

        public const int MaxAvatar = 5;

        public const int HeroLevelMax = 24;

        public const int HeroSkillLevelMax = 3;

        public const string LinkGoogleStore = "https://play.google.com/store/apps/details?id={0}";

        public const string LinkSupport = "http://m.me/empire.defender.official";

        public const string LinkCommunity = "https://www.facebook.com/groups/empiredefender";
        public const string FacebookFanpage = "https://www.facebook.com/empire.defender.official";
        public const string DiscordLink = "https://discord.gg/JCarejsj4b";

        public const float RatioConvertSizeRangeDetect = 2.8f;

        public const int NumberTabShop = 2;

        public const int NumberSlotFullRuneInventory = 200;

        public const int AVAILABLE_AT_STAGE_ID_UNLOCK_GACHA = 7;

        public const string TextColorBrown = "#745f43";

        public const string TextColorOrange = "#bb5c14";

        public const string TextColorGray = "#8c8067";

        public const string TextColorRed = "#e91b24";

        public const int MAX_STAGE_ID_WORLD_1 = 19;

        public const int MAX_SLOT_HERO_DEFENSE = 5;
    }
}
