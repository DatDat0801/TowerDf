using System;

namespace EW2
{
    public class RunePackageUserData : BaseBundleData
    {
        public DateTime lastAutoShow;
        public bool isAutoShown;

        public void SetAutoShow()
        {
            isAutoShown = true;
            lastAutoShow = TimeManager.NowUtc;
        }

        public bool CanAutoShow()
        {
            return CheckCanShow() && isAutoShown == false;
        }
        public override bool CheckCanShow()
        {
            var runePackageData = GameContainer.Instance.Get<ShopDataBase>().Get<RunePackageShopData>();
            var numberStar = UserData.Instance.CampaignData.GetStar(0, runePackageData.unlockConditions[0].mapUnlock);
            var gameLog = UserData.Instance.AccountData.gameLog;
            int purchasedAll = 0;
            for (var i = 0; i < runePackageData.shopItemDatas.Length; i++)
            {
                var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(runePackageData.shopItemDatas[i].productId);
                if (!canBuy)
                {
                    purchasedAll++;
                }
            }

            if (numberStar > 0 && gameLog.openGameCount >= runePackageData.unlockConditions[0].dayOpenGame && purchasedAll < 3)
            {
                // SetTimeStart(TimeManager.NowInSeconds);
                // var endTime = TimeManager.NowInSeconds + runePackageData.unlockConditions[0].duration;
                // SetTimeEnd((long)endTime);
                // UserData.Instance.Save();
                return true;
            }

            return false;

        }
    }
}
