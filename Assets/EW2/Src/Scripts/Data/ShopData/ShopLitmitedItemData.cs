using System;

namespace EW2
{
    [Serializable]
    public class ShopLitmitedItemData
    {
        public string productId;
        public float price;
        public float pricePrevious;
        public int consumable;
        public Reward[] rewards;
        public int limit;

        public Reward[] GenRewards()
        {
            return Reward.GenRewards(this.rewards);
        }
    }
}