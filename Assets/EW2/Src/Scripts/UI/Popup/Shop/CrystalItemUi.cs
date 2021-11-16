using System;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class CrystalItemUi : ShopItemUi
    {
        [SerializeField] private GameObject salePanel;
        [SerializeField] private GameObject tagPanel;
        [SerializeField] private Text txtTitleSale;
        [SerializeField] private Text txtPercentSale;
        [SerializeField] private Text txtTag;
        [SerializeField] private Image imgMoneyToBuy;
        [SerializeField] private Text txtValuePrev;
        private Action<int> OpenGemTab;
        ShopCrystalItemData itemCrytalData;

        public void SetData(ShopCrystalItemData data, Action<int> gemTab = null)
        {
            this.OpenGemTab = gemTab;
            this.itemCrytalData = data;
            ShowUi();
        }
        private void ShowUi()
        {
            txtTitleSale.text = L.shop.sale_txt;
            txtPrice.text = itemCrytalData.costExchange.ToString();
            imgItem.sprite = ResourceUtils.GetSpriteAtlas("shop_icons", $"shop_icon_{itemCrytalData.imgId}");
            imgItem.SetNativeSize();
            txtValue.text = itemCrytalData.valueExchange.ToString();
            if (itemCrytalData.saleType != SaleType.None)
            {
                txtTag.text = GetTagName(itemCrytalData.saleType);
                tagPanel.SetActive(true);
            }
            else
            {
                tagPanel.SetActive(false);
            }
            if (txtValuePrev && Math.Abs(itemCrytalData.valuePrevious - itemCrytalData.valueExchange) > 0)
            {
                txtValuePrev.text = itemCrytalData.valuePrevious.ToString();
                txtValuePrev.gameObject.SetActive(true);
            }
            else
            {
                txtValuePrev.gameObject.SetActive(false);
            }
            if (salePanel)
            {
                if (itemCrytalData.costExchangePrevious <= 0f || Math.Abs(itemCrytalData.costExchangePrevious - itemCrytalData.costExchange) <= 0)
                {
                    salePanel.SetActive(false);
                }
                else
                {
                    float percentSale = 1-itemCrytalData.costExchange/itemCrytalData.costExchangePrevious;
                    txtPercentSale.text = $"{(percentSale * 100):n0}%";
                    salePanel.SetActive(true);
                }
            }

            if (imgMoney)
            {
                imgMoney.sprite = ResourceUtils.GetIconMoneyReward(MoneyType.Crystal);
            }
            if (imgMoneyToBuy)
            {
                imgMoneyToBuy.sprite = ResourceUtils.GetIconMoneyReward(itemCrytalData.moneyTypeExchange);
            }

        }

        protected override void BuyOnClick()
        {
            if (UserData.Instance.GetMoney(itemCrytalData.moneyTypeExchange) < itemCrytalData.costExchange)
            {
                string content = string.Format(L.popup.insufficient_resource, L.currency_type.currency_0) +" "+L.popup.get_more_txt;
                PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(L.popup.notice_txt,content,PopupNoticeWindowProperties.PopupType.TwoOption,L.button.btn_ok,()=>
                {
                    OpenGemTab?.Invoke((int)ShopTabId.Gem);
                },L.button.btn_no,
                    null);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                return;
            }
            UserData.Instance.SubMoney(itemCrytalData.moneyTypeExchange, (long)itemCrytalData.costExchange, AnalyticsConstants.SourceShop, "");
            Reward reward = Reward.Create(ResourceType.Money,MoneyType.Crystal,(int)itemCrytalData.valueExchange);
            PopupUtils.ShowReward(reward);
            reward.AddToUserData();
        }
    }
}
