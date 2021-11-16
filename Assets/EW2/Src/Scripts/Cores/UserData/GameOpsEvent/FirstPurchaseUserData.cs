using System;

namespace EW2
{
    [Serializable]
    public class FirstPurchaseUserData
    {
        public DateTime firstPurchaseTime;
        public bool canClaim;
        public bool isClaimed;
        public DateTime lastAutoShow;
        public bool SetFirstPurchase(DateTime time)
        {
            if (canClaim == false)
            {
                firstPurchaseTime = time;
                canClaim = true;
                return true;
            }

            return false;
        }

        public void SetShowToday(DateTime time)
        {
            lastAutoShow = time;
        }
        public void SetClaimed()
        {
            isClaimed = true;
        }
        public bool IsClaimed
        {
            get => isClaimed;
        }

        public bool CanClaim
        {
            get => canClaim;
        }

        public bool CanShowToday()
        {
            return IsClaimed == false && lastAutoShow.Date != TimeManager.NowUtc.Date;
        }
    }
}