using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class ToastSingleRewardPanelProperties : PanelProperties
    {
        public Reward reward;
        public UnityAction CloseCallBack;
        public ToastSingleRewardPanelProperties(Reward reward, UnityAction CloseCallBack = null)
        {
            this.CloseCallBack = CloseCallBack;
            this.reward = reward;
        }
    }

    /// <summary>
    /// Yes, this panel is there, all the time, just waiting for its moment to shine
    /// </summary>
    public class ToastSingleRewardPanelController : APanelController<ToastSingleRewardPanelProperties>
    {
        [SerializeField] private Transform root;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtDesc;

        private RewardUI rewardUi;

        private const float DelayHide = 1;

        protected override void Awake()
        {
            base.Awake();

            OutTransitionFinished += controller => {
                var toastController = Context.Current.GetService<ToastController>();

                toastController.ShowNotiQueue();
            };

            InTransitionFinished += controller => { StartCoroutine(IDelayHide()); };

            OutTransitionFinished += controller => {
                rewardUi.ReturnPool();
                Properties.CloseCallBack?.Invoke();
            };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SpawnReward();

            txtTitle.text = GetNameReward(Properties.reward.type, Properties.reward.id, Properties.reward.itemType);

            txtDesc.text = GetDescReward(Properties.reward.type, Properties.reward.id, Properties.reward.itemType);
        }

        private void SpawnReward()
        {
            rewardUi = ResourceUtils.GetRewardUi(Properties.reward.type);

            rewardUi.SetData(Properties.reward);

            rewardUi.SetParent(root);
        }

        private IEnumerator IDelayHide()
        {
            yield return new WaitForSecondsRealtime(DelayHide);

            UIFrame.Instance.HidePanel(ScreenIds.single_reward);
        }

        private string GetNameReward(ResourceType type, int id, int itemType)
        {
            if (type == ResourceType.Money)
            {
                return Localization.Current.Get("currency_type", $"currency_{id}");
            }
            else if (type == ResourceType.Inventory)
            {
                if (itemType == InventoryType.Spell)
                    return Ultilities.GetNameSpell(id);
                else if (itemType == InventoryType.SpellFragment)
                    return L.gameplay.common_shard;
                else if (itemType == InventoryType.Rune)
                {
                    return Ultilities.GetNameRune(id);
                }
            }
            else if (type == ResourceType.Hero)
            {
                return Ultilities.GetNameHero(id);
            }

            return "";
        }

        private string GetDescReward(ResourceType type, int id, int itemType)
        {
            if (type == ResourceType.Money)
            {
                return Localization.Current.Get("currency_type", $"currency_{id}_des");
            }
            else if (type == ResourceType.Inventory)
            {
            }
            else if (type == ResourceType.Hero)
            {
                return Ultilities.GetTagHero(id);
            }

            return "";
        }
    }
}
