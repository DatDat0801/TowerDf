using System;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class VideoAdPlayer : Singleton<VideoAdPlayer>
    {
        /// <summary>
        /// bool: lockUI ad click
        /// </summary>
        //public event UnityAction<bool> OnViewAdResult = delegate(bool b) {  };
        public event UnityAction<string> OnRewarded;

        public event UnityAction OnWatchAdFailed;

        private void OnEnable()
        {
            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        }

        private void OnDisable()
        {
            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;
        }

        public void PlayAdClick(string placementName)
        {
            var isTest = GameLaunch.isCheat;

            if (isTest)
            {
                EventManager.EmitEvent(GamePlayEvent.OnWatchVideo);
                OnRewarded?.Invoke(placementName);
                return;
            }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            IronSource.Agent.showRewardedVideo(placementName);
            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var ad = Array.Find(adRewardData.ads, entity => entity.placementName == placementName);
            FirebaseLogic.Instance.AdClick(ad.location, ad.adId.ToString());
#else
            Ultilities.ShowToastNoti(L.popup.no_ads_available_txt);
#endif
        }

        private void RewardedVideoAdClickedEvent(IronSourcePlacement obj)
        {
            Debug.LogWarning("Rewarded Video Ad Clicked Event");
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
        {
            Debug.LogWarning("Rewarded Video Ad Show Failed Event");
            OnWatchAdFailed?.Invoke();
            Ultilities.ShowToastNoti(L.popup.no_ads_available_txt);
        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
        {
            EventManager.EmitEvent(GamePlayEvent.OnWatchVideo);
            OnRewarded?.Invoke(obj.getPlacementName());
            Debug.LogWarning("Rewarded Video Ad Rewarded Event");

            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var ad = Array.Find(adRewardData.ads, entity => entity.placementName == obj.getPlacementName());
            FirebaseLogic.Instance.AdReward(ad.location, ad.adId.ToString());
            AppsflyerUtils.Instance.CompleteAdsEvent(ad.location);
        }

        private void RewardedVideoAdEndedEvent()
        {
        }

        private void RewardedVideoAdStartedEvent()
        {
        }

        private void RewardedVideoAvailabilityChangedEvent(bool obj)
        {
        }

        private void RewardedVideoAdClosedEvent()
        {
            OnWatchAdFailed?.Invoke();
        }

        private void RewardedVideoAdOpenedEvent()
        {
        }
    }
}