using System;

namespace EW2
{
    [Serializable]
    public class Hero4ResourceFlashSaleUserData : BaseBundleData
    {
        //public bool canRemind;
        //public bool dontShowToday;
        public DateTime lastShow;
        /// <summary>
        /// the package already shown and time out
        /// </summary>
        public bool shown;

        public bool CanRemind()
        {
            var hero4BundleData = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            var productId = hero4BundleData.shopItemDatas[0].productId;

            var isHero4BundlePurchased = UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId);
            if ((isHero4BundlePurchased || UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.Marco)) && UserData.Instance.UserEventData.IsAnyFlashSaleOpen() == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
            // var hero4Package = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            // if (!UserData.Instance.UserShopData.CheckPackNonconsumePurchased(hero4Package.shopItemDatas[0].productId))
            // {
            //     return false;
            // }
            if (this.shown) return false;
            var flashSalePackage = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4ResourceFlashSaleData>();
            //var numberStar = UserData.Instance.CampaignData.GetStar(0, starterPackDataBase.packConditions[0].mapUnlock);
            var productId = flashSalePackage.shopItemDatas[0].productId;
            var isPurchased = UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId);
            if (!isOpen)
            {
                if ((CanRemind()) && !isPurchased)
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
                if (TimeRemain() > 0 && !isPurchased)
                {
                    return true;
                }
                else
                {
                    isOpen = false;
                    this.shown = true;
                    UserData.Instance.Save();
                    return false;
                }
            }
        }
        public bool CanRemindHero5()
        {
            if (UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.NeroCat) && UserData.Instance.UserEventData.IsAnyFlashSaleOpen() == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckCanShowFor1005()
        {
            if (this.shown) return false;
            var flashSalePackage = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4ResourceFlashSaleData>();
            //var numberStar = UserData.Instance.CampaignData.GetStar(0, starterPackDataBase.packConditions[0].mapUnlock);
            var productId = flashSalePackage.shopItemDatas[0].productId;
            var isPurchased = UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId);
            if (!isOpen)
            {
                if ((CanRemindHero5()) && !isPurchased)
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEnd = TimeManager.NowInSeconds + flashSalePackage.packConditions[0].duration * 3600;
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
                if (TimeRemain() > 0 && !isPurchased)
                {
                    return true;
                }
                else
                {
                    isOpen = false;
                    this.shown = true;
                    UserData.Instance.Save();
                    return false;
                }
            }
        }
    }
}
