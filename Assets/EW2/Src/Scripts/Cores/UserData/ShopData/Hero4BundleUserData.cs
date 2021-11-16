using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    [Serializable]
    public class Hero4BundleUserData : BaseBundleData
    {
        /// <summary>
        /// Save the auto show hero 4 bundle, depend on pack condition
        /// List of stage shown
        /// </summary>
        public List<int> reminded = new List<int>();


        public void SetReminded(int mapId)
        {
            var campaignId = MapCampaignInfo.GetCampaignId(0, mapId, 0);
            reminded.Add(campaignId);
        }

        public bool IsReminded(int mapId)
        {
            var campaignId = MapCampaignInfo.GetCampaignId(0, mapId, 0);
            return reminded.Contains(campaignId);
        }

        public bool CanAutoShow(int mapId)
        {
            var bundle4Db = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            var packCondition = Array.Find(bundle4Db.packConditions,
                condition => condition.mapUnlock == mapId);
            if (packCondition == null)
            {
                return false;
            }

            //Debug.LogAssertion(CheckCanShow() + "~" + !IsReminded(mapId));
            return CheckCanShow() && !IsReminded(mapId);
        }

        public override bool CheckCanShow()
        {
            var hero4Unlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(1004);
            if (hero4Unlocked) return false;
            
            var bundle4Db = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            var mapUnlock = bundle4Db.packConditions[0].mapUnlock;
            var numberStar = UserData.Instance.CampaignData.GetStar(0, mapUnlock);

            if (!isOpen)
            {
                if (numberStar > 0)
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEnd = TimeManager.NowInSeconds +
                                  bundle4Db.packConditions[0].duration * 3600;
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
                var productId = bundle4Db.shopItemDatas[0].productId;
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