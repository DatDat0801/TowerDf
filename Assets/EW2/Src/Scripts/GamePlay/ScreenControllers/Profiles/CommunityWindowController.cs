using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class CommunityWindowController : AWindowController
    {
        [SerializeField] private Text title;
        [SerializeField] private Text subtitle;
        [SerializeField] private Text fbText;
        [SerializeField] private Text discordTxt;
        [SerializeField] private Text groupTxt;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button facebookBtn;
        [SerializeField] private Button discordBtn;
        [SerializeField] private Button groupBtn;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetLocalization();
            
            this.closeBtn.onClick.AddListener(OnCloseClick);
            this.facebookBtn.onClick.AddListener(OnFacebookClick);
            this.discordBtn.onClick.AddListener(OnDiscordClick);
            this.groupBtn.onClick.AddListener(OnGroupClick);
        }

        public override void SetLocalization()
        {
            base.SetLocalization();
            if (this.title)
            {
                this.title.text = L.social.community_title_txt;
            }

            if (this.subtitle)
            {
                var reward = GameContainer.Instance.GetCommunityReward();
                this.subtitle.text = string.Format(L.social.welcome_to_game_txt, L.common.game_name_txt, reward.joinCommunity.number.ToString());
            }

            if (this.fbText)
            {
                this.fbText.text = L.social.facebook_title;
            }

            if (this.discordTxt)
            {
                this.discordTxt.text = L.social.discord_title;
            }

            if (this.groupTxt)
            {
                this.groupTxt.text = L.social.group_title;
            }
        }

        private void OnGroupClick()
        {
            FirebaseLogic.Instance.ButtonClick("profile", "group", -1);
            Application.OpenURL(GameConfig.LinkCommunity);
            EventManager.EmitEvent(GamePlayEvent.OnJointFanpage);
            var userData = UserData.Instance.UserEventData.CommunityEventUserData;
            var getReward = userData.SetAsClaimedReward(CommunityChannel.FacebookGroup);
            if (getReward)
            {
                var reward = GameContainer.Instance.GetCommunityReward();
                PopupUtils.ShowReward(new []{reward.joinCommunity});
                EventManager.EmitEvent(GamePlayEvent.CLICK_COMMUNITY);
                Reward.AddToUserData(new []{reward.joinCommunity}, "community", "group");
                UserData.Instance.Save();
            }
        }

        private void OnDiscordClick()
        {
            FirebaseLogic.Instance.ButtonClick("profile", "discord", -1);
            Application.OpenURL(GameConfig.DiscordLink);
            var userData = UserData.Instance.UserEventData.CommunityEventUserData;
            var getReward = userData.SetAsClaimedReward(CommunityChannel.Discord);
            if (getReward)
            {
                var reward = GameContainer.Instance.GetCommunityReward();
                PopupUtils.ShowReward(new []{reward.joinCommunity});
                EventManager.EmitEvent(GamePlayEvent.CLICK_COMMUNITY);
                Reward.AddToUserData(new []{reward.joinCommunity}, "community", "discord");
                UserData.Instance.Save();
            }
        }

        private void OnFacebookClick()
        {
            FirebaseLogic.Instance.ButtonClick("profile", "facebook", -1);
            Application.OpenURL(GameConfig.FacebookFanpage);
            var userData = UserData.Instance.UserEventData.CommunityEventUserData;
            var getReward = userData.SetAsClaimedReward(CommunityChannel.Facebook);
            if (getReward)
            {
                var reward = GameContainer.Instance.GetCommunityReward();
                PopupUtils.ShowReward(new []{reward.joinCommunity});
                EventManager.EmitEvent(GamePlayEvent.CLICK_COMMUNITY);
                Reward.AddToUserData(new []{reward.joinCommunity}, "community", "facebook");
                UserData.Instance.Save();
            }
        }

        private void OnCloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseWindow(ScreenIds.comunity_popup);
            
        }
    }
}