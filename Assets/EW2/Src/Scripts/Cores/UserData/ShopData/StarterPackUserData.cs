using System;

namespace EW2
{
    [Serializable]
    public class StarterPackUserData : BaseBundleData
    {
        public override bool CheckCanShow()
        {
            var starterPackDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<StarterPackData>();
            var numberStar = UserData.Instance.CampaignData.GetStar(0, starterPackDataBase.packConditions[0].mapUnlock);

            if (!isOpen)
            {
                if ((UserData.Instance.CampaignData.isCanShowStarterPack || numberStar > 0) &&
                    !UserData.Instance.UserHeroData.CheckHeroUnlocked(1003))
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEnd = TimeManager.NowInSeconds +
                                  starterPackDataBase.packConditions[0].duration * 3600;
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
                var productId = starterPackDataBase.shopItemDatas[0].productId;
                if (TimeRemain() > 0 &&
                    !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId))
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