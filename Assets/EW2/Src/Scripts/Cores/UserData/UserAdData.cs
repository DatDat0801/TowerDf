using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class UserAdData
    {
        public int adId;
        public bool claimedReward;
        public int progress;
        public DateTime lastView;
    }

    public class UserAdDataWrapper
    {
        public List<UserAdData> userAdData;

        public UserAdDataWrapper()
        {
            if (userAdData == null) userAdData = new List<UserAdData>();
        }

        public UserAdData GetUserAdData(int adId)
        {
            var userAd = userAdData.Find(data => data.adId == adId);
            if (userAd == null)
            {
                userAd = new UserAdData(){progress = 0, adId = adId, claimedReward = false, lastView = TimeManager.NowUtc};
                userAdData.Add(userAd);
            }
            return userAd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="maxView"></param>
        /// <returns>can be take reward</returns>
        public bool ViewAd(int adId, int maxView)
        {
            var userAd = userAdData.Find(data => data.adId == adId);
            if (userAd == null)
            {
                userAd = new UserAdData() {progress = 0, adId = adId, claimedReward = false, lastView = TimeManager.NowUtc};
                userAdData.Add(userAd);
            }

            if (userAd.progress > maxView)
            {
                Debug.LogAssertion($"Maximum ads can be viewed for the ad id {userAd.adId}");
                return false;
            }

            if (userAd.lastView.Date != TimeManager.NowUtc.Date)
            {
                userAd.lastView = TimeManager.NowUtc;
                userAd.progress = 1;
                if (userAd.progress >= maxView)
                {
                    userAd.claimedReward = true;
                    return true;
                }
                else
                {
                    userAd.claimedReward = false;
                    return false;
                }
            }

            userAd.lastView = TimeManager.NowUtc;
            userAd.progress++;
            if (userAd.progress >= maxView)
            {
                userAd.claimedReward = true;
                return true;
            }
            else
            {
                userAd.claimedReward = false;
                return false;
            }
        }

        public int RemoveUnUsedAds(AdEntity[] ads)
        {
            int adRemoved = 0;
            for (var i = 0; i < ads.Length; i++)
            {
               adRemoved += userAdData.RemoveAll(data => data.adId == ads[i].adId);
            }

            return adRemoved;
        }

        public void ClearUserDataAd()
        {
            userAdData.Clear();
        }

        public int RemoveAdUserDataNotToday()
        {
            return userAdData.RemoveAll(o => o.lastView.Date!=TimeManager.NowUtc.Date);
        }
        // public void ClaimAdReward(int adId)
        // {
        //     var userAd = userAdData.Find(data => data.adId == adId);
        //     if (userAd == null)
        //     {
        //         userAd = new UserAdData() {progress = 0, adId = adId, claimedReward = false, lastView = TimeManager.NowUtc};
        //     }
        //
        //     userAd.claimedReward = true;
        // }
    }
}