using System;

namespace EW2
{
    [Serializable]
    public class RuneFlashSaleUserData : BaseBundleData
    {
        public DateTime lastShow;

        /// <summary>
        /// Don't show today
        /// </summary>
        public void SetStopAutoShow()
        {
            //dontShowToday = true;
            lastShow = TimeManager.NowUtc;
        }

        public bool CanAutoShow()
        {
            return lastShow.Date != TimeManager.NowUtc.Date && CheckCanShow();
        }

        public override bool CheckCanShow()
        {
            var flashSalePackage = GameContainer.Instance.Get<ShopDataBase>().Get<RuneFlashSaleData>();
            var playedStage = UserData.Instance.CampaignData.GetHighestPlayedStage();
            var productId = flashSalePackage.shopItemDatas[0].productId;
            var isPurchased = UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId);
            if (!isOpen && playedStage >= flashSalePackage.packConditions[0].mapUnlock)
            {
                if (UserData.Instance.UserEventData.IsAnyFlashSaleOpen() == false && !isPurchased)
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEnd = TimeManager.NowInSeconds +
                                  flashSalePackage.packConditions[0].duration * 3600;
                    SetTimeEnd(timeEnd);
                    UserData.Instance.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //var productId = flashSalePackage.shopItemDatas[0].productId;
                if (TimeRemain() > 0 && !isPurchased)
                {
                    return true;
                }
                else
                {
                    isOpen = false;
                    UserData.Instance.Save();
                    return false;
                }
            }
        }
    }
}