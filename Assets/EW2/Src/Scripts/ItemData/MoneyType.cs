namespace EW2
{
    public enum ResourceType
    {
        None = -1,
        Money = 0,
        MoneyInGame = 1,
        Inventory = 2,
        Hero = 3,

        //Use for display only, not save to user data
        DFSP = 4,
        GiftCode = 5
    }

    public class MoneyInGameType
    {
        public const int None = -1;
        public const int Gold = 0;
        public const int LifePoint = 1;
    }

    public static class MoneyType
    {
        public const int None = -1;
        public const int Diamond = 0;
        public const int Crystal = 1;
        public const int Exp = 2;
        public const int Stamina = 3;
        /// <summary>
        /// A currency in tournament
        /// </summary>
        public const int GrandMaster = 4;
        public const int KeySpellBasic = 5;
        public const int KeySpellPremium = 6;
        public const int VipPoint = 7;
        public const int SliverStar = 8;
        public const int GoldStar = 9;
        public const int SkillPoint = 10;
        public const int KeyRuneBasic = 11;
        public const int KeyRunePremium = 12;
        public const int ExpRune = 13;
        public const int QuestActivityPoint = 14;
        public const int GloryRoadPoint = 15;
        public const int TournamentTicket = 16;
    }

    public static class InventoryType
    {
        public const int None = -1;
        public const int Spell = 0;
        public const int Rune = 1;
        public const int SpellFragment = 2;
        public const int RandomRune0 = 3;
        public const int RandomRune1 = 4;
        public const int RandomRune2 = 5;
        public const int RandomRune3 = 6;
        public const int RandomRune4 = 7;
        public const int SpellSpecial = 8;
        public const int RandomSpell0 = 9;
        public const int RandomSpell1 = 10;
        public const int RandomSpell2 = 11;
        public const int RandomSpell3 = 12;
        public const int RandomSpell4 = 13;
        public const int RandomSpecialSpell = 14;
        public const int RandomSpecialRune = 15;
    }

    public static class GiftCodeType
    {
        public const int UnlockGloryRoad = 0;
        public const int ReceiveBuyNow = 1;
    }
}