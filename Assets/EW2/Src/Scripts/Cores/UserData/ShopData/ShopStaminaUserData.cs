namespace EW2
{
    public class ShopStaminaUserData : BaseBundleData
    {
        public int stackBuy;
        public int watchAdsCount;
        public void ResetStackBuy()
        {
            stackBuy = 0;
        }

        public void AddStackBuy()
        {
            stackBuy++;
        }

        internal bool CheckCanWatchAds()
        {
            var shopStaminaDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<ShopStaminaData>();
            return watchAdsCount < shopStaminaDataBase.shopStaminaDataBases[0].adsCount;
        }

        internal void BuyByWatchAds()
        {
            watchAdsCount++;
        }
        public override long TimeRemain()
        {
            var timeCurrent = TimeManager.NowInSeconds;
            var timeRemain  = timeEnd - timeCurrent;
            if (timeRemain < 0)
            {
                timeRemain = 1;
                ResetStackBuy();
            }
            return timeRemain;
        }
        internal void SetTime()
        {
            var shopStaminaDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<ShopStaminaData>();
            SetTimeStart(TimeManager.NowInSeconds);
            var timeEnd = TimeManager.NowInSeconds +
                                  shopStaminaDataBase.shopStaminaDataBases[0].timeCountdown * 3600;
            SetTimeEnd(timeEnd);
            UserData.Instance.Save();
        }

        internal void ResetAdsCount()
        {
            watchAdsCount = 0;
        }
    }
}
