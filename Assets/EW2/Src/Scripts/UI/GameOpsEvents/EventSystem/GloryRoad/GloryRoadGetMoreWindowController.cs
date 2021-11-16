using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class GetMoreProperties : WindowProperties
    {
        public TabsManager heroAcademyTab;
        public GetMoreProperties(TabsManager tabsManager)
        {
            this.heroAcademyTab = tabsManager;
        }
    }
    public class GloryRoadGetMoreWindowController : AWindowController<GetMoreProperties>
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text sloganText;
        [SerializeField] private Text dailyGiftText;
        [SerializeField] private Text buyNowText;
        [SerializeField] private Text heroChallengeText;
        [SerializeField] private Button dailyGiftButton;
        [SerializeField] private Button buyNowButton;
        [SerializeField] private Button heroChallengeButton;

        [SerializeField] private Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            dailyGiftButton.onClick.AddListener(DailyGiftClick);
            buyNowButton.onClick.AddListener(BuyNow);
            heroChallengeButton.onClick.AddListener(HeroChallengeClick);
            closeButton.onClick.AddListener(CloseClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            titleText.text = L.popup.notice_txt;
            sloganText.text = string.Empty;
            dailyGiftText.text = L.game_event.event_daily_reward_title_txt;
            buyNowText.text = L.popup.buy_now_title_txt;
            heroChallengeText.text = L.game_event.hero_chalenge_txt;
            
            SetStateButtons();
        }

        private void SetStateButtons()
        {
            var existBuyNowPackage = UserData.Instance.UserEventData.BuyNowUserData.CheckCanShow();
            if (existBuyNowPackage)
            {
                buyNowButton.interactable = true;
            }
            else
            {
                buyNowButton.interactable = false;
            }
            buyNowButton.GetComponentInChildren<Text>().text = L.button.go_to_btn;
            dailyGiftButton.GetComponentInChildren<Text>().text = L.button.go_to_btn;
            heroChallengeButton.GetComponentInChildren<Text>().text = L.button.go_to_btn;
        }
        private void DailyGiftClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
            Properties.heroAcademyTab.SetSelected((int)HeroAcademyTab.DailyGift);
        }

        private void BuyNow()
        {
            UIFrame.Instance.CloseCurrentWindow();
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
        }

        private void HeroChallengeClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
            Properties.heroAcademyTab.SetSelected((int)HeroAcademyTab.HeroChallenge);
        }
        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }
    }
}