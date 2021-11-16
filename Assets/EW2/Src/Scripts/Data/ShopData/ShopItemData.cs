using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    [System.Serializable]
    public class ShopItemData
    {
        public string productId;
        public float price;
        public float pricePrevious;
        public int consumable;
        public int vipPoint;
        public Reward[] rewards;
        public int imgId;
        public int moneyType;
        public SaleType saleType;
        public int valuePrevious;

        public Reward[] GenRewards()
        {
            return Reward.GenRewards(this.rewards);
        }

        public void ConvertDataToLimitedBundle(ShopLitmitedItemData itemData)
        {
            this.productId = itemData.productId;
            this.price = itemData.price;
            this.pricePrevious = itemData.pricePrevious;
            this.consumable = itemData.consumable;
            this.rewards = itemData.rewards;
        }
    }

    public class ShopData : ScriptableObject
    {
        public ShopItemData[] shopItemDatas;
    }
}