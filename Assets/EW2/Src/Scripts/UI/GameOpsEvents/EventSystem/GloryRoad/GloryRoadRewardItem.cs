using Sirenix.OdinInspector;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class GloryRoadRewardItem : MonoBehaviour
    {
        [SerializeField] private GameObject glowEffect;
        [SerializeField] private GameObject sparkEffect;

        [SerializeField] private Image lockIcon;

        [SerializeField] private Image disableHover;
        [SerializeField] private GameObject takenIcon;
        [SerializeField] private RectTransform itemContainer;

        [SerializeField] private Button claimRewardButton;

        public RewardUI FreeRewardUI { get; private set; }
        public RewardUI PremiumRewardUI { get; private set; }

        public int TierId { get; private set; }

        public void RepaintFreeItem(GloryRoadTierItem gloryRoadTierItem, GloryRoadUserData userData)
        {
            TierId = gloryRoadTierItem.tierId;
            if (userData.CanClaimFreeReward(TierId))
            {
                glowEffect.SetActive(true);
            }
            else
            {
                glowEffect.SetActive(false);
            }

            if (gloryRoadTierItem.IsCriticalFreeReward())
            {
                sparkEffect.SetActive(true);
            }
            else
            {
                sparkEffect.SetActive(false);
            }

            var taken = userData.IsFreeRewardTaken(TierId);
            var unlocked = userData.IsTierUnlocked(TierId);
            if (taken)
            {
                takenIcon.SetActive(true);
                disableHover.gameObject.SetActive(true);
            }
            else
            {
                takenIcon.SetActive(false);
                disableHover.gameObject.SetActive(false);
            }

            if (unlocked && !taken)
            {
                disableHover.gameObject.SetActive(false);
            }
            else
            {
                disableHover.gameObject.SetActive(true);
            }

            if (itemContainer != null)
            {
                gameObject.SetActive(true);
                //reward
                var reward = gloryRoadTierItem.GetFreeReward();
                if (reward != null)
                {
                    FreeRewardUI = ResourceUtils.GetRewardUi(reward.type);

                    FreeRewardUI.SetData(reward);
                    FreeRewardUI.SetParent(itemContainer);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }


            claimRewardButton.gameObject.SetActive(true);
            claimRewardButton.onClick.RemoveAllListeners();
            if (userData.CanClaimFreeReward(TierId))
            {
                claimRewardButton.gameObject.SetActive(true);
                claimRewardButton.onClick.AddListener(() => OnClaimFreeReward(gloryRoadTierItem.GetFreeReward()));
            }
            else
            {
                claimRewardButton.gameObject.SetActive(false);
            }
        }

        public void RepaintPremiumItem(GloryRoadTierItem gloryRoadTierItem, GloryRoadUserData userData)
        {
            TierId = gloryRoadTierItem.tierId;
            if (userData.CanClaimPremiumReward(TierId))
            {
                glowEffect.SetActive(true);
            }
            else
            {
                glowEffect.SetActive(false);
            }

            if (gloryRoadTierItem.IsCriticalPremiumReward())
            {
                sparkEffect.SetActive(true);
            }
            else
            {
                sparkEffect.SetActive(false);
            }

            var taken = userData.IsPremiumRewardTaken(TierId);
            var unlocked = userData.IsTierUnlocked(TierId);
            if (taken)
            {
                takenIcon.SetActive(true);
                disableHover.gameObject.SetActive(true);
            }
            else
            {
                takenIcon.SetActive(false);
                disableHover.gameObject.SetActive(false);
            }

            if (unlocked && !taken)
            {
                disableHover.gameObject.SetActive(false);
            }
            else
            {
                disableHover.gameObject.SetActive(true);
            }


            //reward
            //var isExistBuyNow = UserData.Instance.UserEventData.BuyNowUserData.CheckCanShow();
            var hero1003Unlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(1003);
            var heroSourceFromGloryRoad = UserData.Instance.UserEventData.GloryRoadUser.getHeroFromGloryRoad;

            var reward = gloryRoadTierItem.GetPremiumReward(!hero1003Unlocked || heroSourceFromGloryRoad);
            if (reward != null)
            {
                PremiumRewardUI = ResourceUtils.GetRewardUi(reward[0].type);

                PremiumRewardUI.SetData(reward[0]);
                PremiumRewardUI.SetParent(itemContainer);
            }


            if (userData.IsUnlockPremium())
            {
                lockIcon.gameObject.SetActive(false);
            }
            else
            {
                lockIcon.gameObject.SetActive(true);
            }


            claimRewardButton.gameObject.SetActive(true);
            claimRewardButton.onClick.RemoveAllListeners();
            var isUnlock = userData.IsUnlockPremium();
            var canClaim = userData.CanClaimPremiumReward(TierId);
            if (isUnlock && canClaim)
            {
                claimRewardButton.gameObject.SetActive(true);
                var rewards = gloryRoadTierItem.GeneratePremiumRewards(!hero1003Unlocked || heroSourceFromGloryRoad);
                claimRewardButton.onClick.AddListener(() =>
                    OnClaimPremiumReward(rewards));
            }
            else if (isUnlock && canClaim == false)
            {
                claimRewardButton.gameObject.SetActive(false);
            }
            else
            {
                claimRewardButton.gameObject.SetActive(true);
                claimRewardButton.onClick.AddListener(OnLockItemClick);
            }
        }

        void OnClaimFreeReward(Reward reward)
        {
            PopupUtils.ShowReward(reward);
            Reward.AddToUserData(new[] { reward }, AnalyticsConstants.SourceGloryRoadBasic, $"{TierId.ToString()}");
            var gloryUserData = UserData.Instance.UserEventData.GloryRoadUser;

            gloryUserData.SetClaimFreeReward(TierId);
            UserData.Instance.Save();

            EventManager.EmitEvent(GamePlayEvent.OnRepaintGloryRoad);
        }

        void OnClaimPremiumReward(Reward[] rewards)
        {
            PopupUtils.ShowReward(rewards);
            Reward.AddToUserData(rewards, AnalyticsConstants.SourceGloryRoadPremium, $"{TierId.ToString()}");
            var gloryUserData = UserData.Instance.UserEventData.GloryRoadUser;
            gloryUserData.SetClaimPremiumReward(TierId);
            //var hero1003Unlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(1003);
            if (TierId == 1 && rewards[0].type == ResourceType.Hero)
            {
                UserData.Instance.UserEventData.GloryRoadUser.SetGetHeroFromThis();
            }

            UserData.Instance.Save();

            EventManager.EmitEvent(GamePlayEvent.OnRepaintGloryRoad);
        }

        void OnLockItemClick()
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.game_event.glory_road_premium_warning_txt);
        }

        public void AddListinerForItem(UnityAction action)
        {
            claimRewardButton.onClick.AddListener(action);
        }
        [Button]
        private void FakeClaim()
        {
            claimRewardButton.onClick?.Invoke();
        }
    }
}
