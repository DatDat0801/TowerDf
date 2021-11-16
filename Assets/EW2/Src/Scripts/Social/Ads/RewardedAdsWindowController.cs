using System;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class RewardedAdsWindowController : AWindowController
    {
        [SerializeField] private AdTabUI[] tabUis;
        [SerializeField] private TimeRemainUi timer;
        [SerializeField] private Button closeButton;

        private CanvasGroup CanvasGroup
        {
            get => GetComponent<CanvasGroup>();
        }

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            foreach (var tab in tabUis)
            {
                tab.OnAdClick = OnAdClick;
            }
        }

        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += RewardedAdSystem;
            VideoAdPlayer.Instance.OnWatchAdFailed += HandleOnPlayAdFailed;
        }

        private void OnDisable()
        {
            VideoAdPlayer.Instance.OnRewarded -= RewardedAdSystem;
            VideoAdPlayer.Instance.OnWatchAdFailed -= HandleOnPlayAdFailed;
        }


        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        private void SetTimer()
        {
            var utcNow = TimeManager.NowUtc;
            var remainingTime = new TimeSpan(1, 0, 0, 0, 0) -
                                new TimeSpan(0, utcNow.Hour, utcNow.Minute, utcNow.Second, utcNow.Millisecond);
            timer.SetTimeRemain((long) remainingTime.TotalSeconds, TimeRemainFormatType.Hhmmss,
                delegate
                {
                    UserData.Instance.UserAdDataWrapper.ClearUserDataAd();
                    UserData.Instance.Save();
                    Repaint();
                });
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            Repaint();
        }

        private void SetInteractable(bool interactable)
        {
            CanvasGroup.interactable = interactable;
            Repaint();
        }

        public void Repaint()
        {
            var userAdWrapper = UserData.Instance.UserAdDataWrapper;

            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            //use ad id 0 => 4
            for (var i = 0; i < 4; i++)
            {
                var userAdData = userAdWrapper.GetUserAdData(i);
                bool available = true;
                if (userAdData.adId > 0)
                {
                    var previousUserAdData = userAdWrapper.GetUserAdData(i - 1);
                    available = previousUserAdData.claimedReward ? true : false;
                }

                tabUis[i].Repaint(adRewardData.GetAdEntity(i), userAdData, available);
            }

            if (AdRewardPrompt.CheckNotice())
            {
                EventManager.EmitEventData(GamePlayEvent.VIEW_AD_VIDEO, true);
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.VIEW_AD_VIDEO, false);
            }

            SetTimer();
        }

        /// <summary>
        /// Handle on complete ad system (4 ads)
        /// </summary>
        void RewardedAdSystem(string placementName)
        {
            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var ad = Array.Find(adRewardData.ads, entity => entity.placementName == placementName);

            if (ad != null)
            {
                var userDataWrapper = UserData.Instance.UserAdDataWrapper;
                var canBeClammed = userDataWrapper.ViewAd(ad.adId, ad.adQuantity);
                UserData.Instance.Save();
                if (!canBeClammed)
                {
                    //Debug.LogAssertion("fail to set user data view ad");
                    SetInteractable(true);
                    return;
                }
                //userDataWrapper.ClaimAdReward(ad.adId);

                if (ad.rewards.Length == 1)
                {
                    PopupUtils.ShowReward(ad.rewards);
                }
                else if (ad.rewards.Length > 1)
                {
                    PopupUtils.ShowReward(ad.rewards);
                }

                Reward.AddToUserData(ad.rewards);
            }
            SetInteractable(true);
        }

        void HandleOnPlayAdFailed()
        {
            SetInteractable(true);
        }

        void OnAdClick()
        {
            CanvasGroup.interactable = false;
        }
    }
}