using System;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.Localization;

namespace EW2
{
    public class DefensiveBuffItemUI : MonoBehaviour
    {
        [SerializeField] private Image buffIcon;
        [SerializeField] private Text countText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Text priceText;
        [SerializeField] private Text buffNameText;

        public UnityAction<DefensiveBuffItemUI> OnBuyItem { get; set; }
        public BuffItem BuffItem { get; private set; }

        public void BuyClick()
        {
            OnBuyItem?.Invoke(this);
        }

        public void DisableBuy(bool canBuy)
        {
            var item = GetComponentsInChildren<UIEffect>();
            foreach (UIEffect uiEffect in item)
            {
                uiEffect.enabled = !canBuy;
            }
            this.buyButton.interactable = canBuy;
        }
        public void Repaint(BuffItem buffItem)
        {
            this.BuffItem = buffItem;


            if (this.countText)
            {
                this.countText.text = buffItem.BuffQuantity.ToString();
            }

            if (this.descriptionText)
            {
                string des =
                    string.Format(Localization.Current.Get("playable_mode", $"buff_des_{buffItem.BuffData.buffId}"),
                        buffItem.BuffData.GetDescStatSkillActive().ToArray());
                this.descriptionText.text = des;
            }

            if (this.buyButton)
            {
                this.buyButton.onClick.RemoveAllListeners();
                this.buyButton.onClick.AddListener(BuyClick);
            }

            if (this.priceText)
            {
                this.priceText.text = buffItem.Price.ToString();
            }

            if (this.buffNameText)
            {
                this.buffNameText.text =
                    Localization.Current.Get("playable_mode", $"buff_name_{buffItem.BuffData.buffId}");
            }

            if (this.buffIcon)
            {
                this.buffIcon.sprite =
                    ResourceUtils.GetSpriteAtlas("Buff_icons", $"buff_icon_{buffItem.BuffData.buffId}");
            }
        }
    }
}