using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShopItemUi : MonoBehaviour
    {
        [SerializeField] protected Image imgItem;
        [SerializeField] protected Image imgMoney;
        [SerializeField] protected Text txtPrice;
        [SerializeField] protected Text txtValue;
        [SerializeField] protected Button btnBuy;

        protected ShopItemData itemData;

        private void Awake()
        {
            if (btnBuy)
                btnBuy.onClick.AddListener(BuyOnClick);
        }

        public virtual void SetData(ShopItemData data)
        {
            this.itemData = data;
        }

        public virtual void ReloadData()
        {
        }

        protected virtual void BuyOnClick()
        {
        }

        protected string GetTagName(SaleType type)
        {
            switch (type)
            {
                case SaleType.Hot:
                    return L.shop.hot_deal_txt;
                case SaleType.Best:
                    return L.shop.best_deal_txt;
                default:
                    return String.Empty;
            }
        }
    }
}