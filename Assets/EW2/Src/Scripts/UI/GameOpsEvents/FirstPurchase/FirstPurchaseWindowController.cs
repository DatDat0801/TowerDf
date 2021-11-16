using System.Collections.Generic;
using System.Linq;
using EW2.Tools;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class FirstPurchaseWindowController : AWindowController
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button buyLinkButton;
        [SerializeField] private RectTransform rewardContainer;

        private List<RewardUI> rewardUIs;

        public List<RewardUI> RewardUIs
        {
            get
            {
                if (rewardUIs == null)
                {
                    rewardUIs = new List<RewardUI>();
                }

                return rewardUIs;
            }
            set { rewardUIs = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            claimButton.onClick.AddListener(ClaimClick);
            buyLinkButton.onClick.AddListener(BuyClick);
        }

        private void BuyClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
            if (UserData.Instance.UserEventData.BuyNowUserData.CheckCanShow())
            {
                UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
            }
            else
            {
                UIFrame.Instance.OpenWindow(ScreenIds.shop_scene);
            }
        }

        private void ClaimClick()
        {
            var userEventData = UserData.Instance.UserEventData;
            var canClaim = userEventData.FirstPurchase.CanClaim;
            if (canClaim == false)
            {
                return;
            }
            // UIFrame.Instance.CloseCurrentWindow();
            UIFrame.Instance.CloseWindow(ScreenIds.first_purchase_popup);

            var rewards = GameContainer.Instance.GetFirstPurchaseReward();
            PopupUtils.ShowReward(rewards.rewards.ToArray());
            Reward.AddToUserData(rewards.rewards.ToArray());
            

            EventManager.EmitEvent(GamePlayEvent.FIRST_PURCHASE_CLAIMED);
            SaveClaimedState();
            
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        void SaveClaimedState()
        {
            UserData.Instance.SetClaimedFirstPurchase();
        }

        void SetButtonState()
        {
            var userEventData = UserData.Instance.UserEventData;
            var canClaim = userEventData.FirstPurchase.CanClaim;

            claimButton.gameObject.SetActive(canClaim);
            buyLinkButton.gameObject.SetActive(!canClaim);
            if (!canClaim)
            {
                var canShowBuyNow = UserData.Instance.UserEventData.StarterPackUserData.CheckCanShow();
                if (canShowBuyNow)
                {
                    buyLinkButton.GetComponentInChildren<Text>().text = L.button.go_to_shop_btn;
                }
                else
                {
                    buyLinkButton.GetComponentInChildren<Text>().text = L.button.go_to_shop_btn;
                }
            }
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            // foreach (Transform rewardUi in rewardContainer)
            // {
            //     LeanPool.Despawn(rewardUi.gameObject);
            // }
            rewardContainer.DestroyAllChildren();
            var rewards = GameContainer.Instance.GetFirstPurchaseReward();
            for (var i = 0; i < rewards.rewards.Length; i++)
            {
                var rewardUi = ResourceUtils.GetRewardUi(rewards.rewards[i].type);
                rewardUi.SetData(rewards.rewards[i]);
                rewardUi.SetParent(rewardContainer);
                RewardUIs.Add(rewardUi);
            }

            SetButtonState();
        }
    }
}