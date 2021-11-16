using System;

namespace EW2
{
    [Serializable]
    public class SpellPackageUserData : BaseBundleData
    {
        public DateTime lastShow;
        public bool isAutoShown;
        /// <summary>
        /// Don't show today
        /// </summary>
        public void SetStopAutoShow()
        {
            //dontShowToday = true;
            isAutoShown = true;
            lastShow = TimeManager.NowUtc;
        }
        public bool CanAutoShow()
        {
            return lastShow.Date != TimeManager.NowUtc.Date && CheckCanShow() && isAutoShown == false;
        }
        public override bool CheckCanShow()
        {
            var spellPackageData = GameContainer.Instance.Get<ShopDataBase>().Get<SpellpackageData>();
            var numberStar = UserData.Instance.CampaignData.GetStar(0, spellPackageData.unlockConditions[0].mapUnlock);
            var gameLog = UserData.Instance.AccountData.gameLog;
            int purchasedPack = 0;
            for (var i = 0; i < spellPackageData.shopItemDatas.Length; i++)
            {
                var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(spellPackageData.shopItemDatas[i].productId);
                if (!canBuy)
                {
                    purchasedPack++;
                }
            }
            if (numberStar > 0
                && gameLog.openGameCount >= spellPackageData.unlockConditions[0].dayOpenGame
                && purchasedPack < 3
                )
            {
                return true;

            }
            return false;
        }
    }
}
