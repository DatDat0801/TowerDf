using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class GemItemUi : ShopItemUi
    {
        [SerializeField] private GameObject salePanel;
        [SerializeField] private GameObject tagPanel;
        [SerializeField] private Text txtTitleSale;
        [SerializeField] private Text txtPercentSale;
        [SerializeField] private Text txtTag;
        [SerializeField] private Text txtValuePrev;

        public override void SetData(ShopItemData data)
        {
            base.SetData(data);

            ShowUi();
        }

        private void ShowUi()
        {
            txtTitleSale.text = L.shop.sale_txt;

            imgItem.sprite = ResourceUtils.GetSpriteAtlas("shop_icons", $"shop_icon_{itemData.imgId}");
            imgItem.SetNativeSize();

            if (itemData.saleType != SaleType.None)
            {
                txtTag.text = GetTagName(itemData.saleType);
                tagPanel.SetActive(true);
            }
            else
            {
                tagPanel.SetActive(false);
            }

            if (salePanel)
            {
                if (itemData.pricePrevious <= 0f || Math.Abs(itemData.pricePrevious - itemData.price) <= 0)
                {
                    salePanel.SetActive(false);
                }
                else
                {
                    var percentSale = itemData.price / itemData.pricePrevious;
                    txtPercentSale.text = $"{(percentSale * 100):n0}%";
                    salePanel.SetActive(true);
                }
            }

            if (txtValue && itemData.rewards.Length > 0)
            {
                txtValue.text = itemData.rewards[0].number.ToString();
            }

            if (txtValuePrev && Math.Abs(itemData.rewards[0].number - itemData.valuePrevious) > 0)
            {
                txtValuePrev.text = itemData.valuePrevious.ToString();
                txtValuePrev.gameObject.SetActive(true);
            }
            else
            {
                txtValuePrev.gameObject.SetActive(false);
            }

            if (txtPrice)
            {
                var price = ProductsManager.GetLocalPriceStringById(itemData.productId);


                if (price.Equals("$0.01") || string.IsNullOrEmpty(price))
                {
                    price = $"${itemData.price}";
                }

                txtPrice.text = price;
            }

            if (imgMoney)
            {
                imgMoney.sprite = ResourceUtils.GetIconMoneyReward(itemData.moneyType);
            }
        }

        protected override void BuyOnClick()
        {
            ShopService.Instance.BuyGem(itemData, HandleBuySucess);
        }

        private void HandleBuySucess(bool result, Reward[] gifts)
        {
            if (result)
            {
                PopupUtils.ShowReward(gifts);
            }
        }
    }
}
