using System.Collections.Generic;

namespace EW2.Spell
{
    public sealed class UnlockFeatureUtilities
    {
        //private List<int> lstSpellCanUpgrade = new List<int>();
        public const int SPELL_AVAILABLE_AT_STAGE_ID = 9;
        public const int RUNE_AVAILABLE_AT_STAGE_ID = 7;

        /// <summary>
        /// Availability of spell feature
        /// </summary>
        /// <returns></returns>
        public static bool IsSpellAvailable()
        {
            var campaignData = UserData.Instance.CampaignData;
            var unlockedStage = campaignData.HighestPassLevel();
            return SPELL_AVAILABLE_AT_STAGE_ID < unlockedStage;
        }

        public static bool IsRuneAvailable()
        {
            var campaignData = UserData.Instance.CampaignData;
            var unlockedStage = campaignData.HighestPassLevel();
            return RUNE_AVAILABLE_AT_STAGE_ID < unlockedStage;
        }

        public static bool IsCanClaimHero1003Free()
        {
            var eventHeroAcademyEnable = UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow();
            var isUnlocked1003 = UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.Neetan);
            var buyNowData = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();
            var isPurchaseBuyNow =
                UserData.Instance.UserShopData.CheckPackNonconsumePurchased(buyNowData.shopItemDatas[0].productId);

            return eventHeroAcademyEnable && !isUnlocked1003 && isPurchaseBuyNow;
        }
    }
}