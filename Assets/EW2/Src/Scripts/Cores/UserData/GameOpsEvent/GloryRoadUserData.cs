using System;
using System.Collections.Generic;
using System.Linq;

namespace EW2
{
    [Serializable]
    public class GloryRoadUserData
    {
        //public int gloryRoadPoint;
        //public int unlockedTier;
        public int[] clammedFreeRewards;
        public int[] clammedPremiumRewards;
        public string premiumKey;
        public bool getHeroFromGloryRoad;
        public GloryRoadUserData()
        {
            if (clammedFreeRewards == null)
            {
                clammedFreeRewards = new int[11];
            }

            if (clammedPremiumRewards == null)
            {
                clammedPremiumRewards = new int[11];
            }
        }

        public void SetGetHeroFromThis()
        {
            getHeroFromGloryRoad = true;
        }
        
        public bool IsTierUnlocked(int tierId)
        {
            return tierId <= UnlockedTier();
        }

        public bool CanClaimFreeReward(int tierId)
        {
            return clammedFreeRewards[tierId - 1] == 0 && UnlockedTier() >= tierId;
        }

        public bool CanClaimPremiumReward(int tierId)
        {
            return clammedPremiumRewards[tierId - 1] == 0 && UnlockedTier() >= tierId;
        }
        public bool CanClaimAnyReward()
        {
            for (var i = 0; i < 8; i++)
            {
                if (clammedFreeRewards[i] == 0 && UnlockedTier() >= i + 1)
                {
                    return true;
                }
            }

            if (!IsUnlockPremium())
            {
                return false;
            }
            for (var i = 0; i < 11; i++)
            {
                if (clammedPremiumRewards[i] == 0 && UnlockedTier() >= i + 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetClaimFreeReward(int tierId)
        {
            clammedFreeRewards[tierId - 1] = 1;
        }
        public void SetClaimPremiumReward(int tierId)
        {
            clammedPremiumRewards[tierId - 1] = 1;
        }

        public void SetPremiumKey(string generatedKey)
        {
            premiumKey = generatedKey;
        }

        public bool IsUnlockPremium()
        {
            var buyNowDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();
            var productId = buyNowDataBase.shopItemDatas[0].productId;
            return !string.IsNullOrEmpty(premiumKey) || UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId);
        }

        public bool IsFreeRewardTaken(int tierId)
        {
            if (tierId > UnlockedTier())
            {
                return false;
            }

            return clammedFreeRewards[tierId - 1] > 0;
        }

        public bool IsPremiumRewardTaken(int tierId)
        {
            if (tierId > UnlockedTier())
            {
                return false;
            }

            return clammedPremiumRewards[tierId - 1] > 0;
        }

        public int GetUserPointForTier(GloryRoadTierItem[] totalNeededPoints, int forTier)
        {
            var points = new List<int>();
            for (var i = 0; i < totalNeededPoints.Length; i++)
            {
                points.Add(totalNeededPoints[i].point);
            }
            if (forTier <= UnlockedTier())
            {
                return totalNeededPoints[forTier - 1].point;
            }

            var sum = points.Take(forTier - 1).Sum();
            var gloryRoadPoint = UserData.Instance.GetMoney(MoneyType.GloryRoadPoint);
            return (int)gloryRoadPoint - sum;
        }
        // public int GetUserPointForCurrentTier(GloryRoadTierItem[] totalNeededPoints)
        // {
        //     var points = new List<int>();
        //     for (var i = 0; i < totalNeededPoints.Length; i++)
        //     {
        //         points.Add(totalNeededPoints[i].point);
        //     }
        //     
        //     var sum = points.Take(UnlockedTier()).Sum();
        //     var gloryRoadPoint = UserData.Instance.GetMoney(MoneyType.GloryRoadPoint);
        //     return (int) gloryRoadPoint - sum;
        // }
        public int UnlockedTier()
        {
            var gloryRoadPoint = UserData.Instance.GetMoney(MoneyType.GloryRoadPoint);
            var totalNeededPoints = GameContainer.Instance.GetGloryRoadData().items;
            int tierUnlocked = 0;
            //int sum = 0;
            // var index = Array.FindIndex(totalNeededPoints, item => gloryRoadPoint >= item.point);
            // if (index == -1)
            // {
            //     return totalNeededPoints.Length;
            // }
            //
            // return index + 1;
             foreach (var t in totalNeededPoints)
             {
                 //sum += t.point;
                 if (t.point <= gloryRoadPoint)
                 {
                     tierUnlocked++;
                 }
                 else
                 {
                     break;
                 }
             }

            return tierUnlocked;
        }
        public int GetTierCanClaimReward()
        {
            var unlockedTier = UnlockedTier();
            var tierCanClaims = new List<int>();
            for (var i = 0; i < 8; i++)
            {
                if (clammedFreeRewards[i] == 0 &&  unlockedTier >= i + 1)
                {
                    tierCanClaims.Add(i + 1) ;
                }
            }

            if (!IsUnlockPremium())
            {
                tierCanClaims.Add(unlockedTier); 
            }
            else
            {
                for (var i = 0; i < 11; i++)
                {
                    if (clammedPremiumRewards[i] == 0 && unlockedTier >= i + 1)
                    {
                        tierCanClaims.Add(i+1);
                    }
                }
            }

            if (tierCanClaims.Count <= 0)
            {
                return unlockedTier;
            }
            return tierCanClaims.Min();
        }
    }
}