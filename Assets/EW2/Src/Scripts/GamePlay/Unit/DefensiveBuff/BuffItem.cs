using System;
using UnityEngine;

namespace EW2
{
    public class BuffItem : IBuffItem
    {
        public BuffBase BuffData { get; private set; }
        public int BuffQuantity { get; private set; }
        public int Price { get; private set; }

        public IBuffItem ConstructFromUserData(DefensiveBuffUserData userData)
        {
            var result = userData.UserBuff.TryGetValue(BuffData.buffId, out int value);
            if (result == false)
            {
                //Debug.LogError($"Call {nameof(ConstructBuffStats)} first");
                return this;
            }

            BuffQuantity = value;
            return this;
        }

        public IBuffItem ConstructBuffStats(BuffBase dataBase)
        {
            BuffData = dataBase;
            return this;
        }

        public IBuffItem ConstructPrice(BuffExchangeDatabase exchangeDatabase)
        {
            var exchange = Array.Find(exchangeDatabase.exchangeRates, rate => rate.buffId == BuffData.buffId);

            if (exchange == null)
            {
                Debug.LogError($"Call {nameof(ConstructBuffStats)} first");
                return this;
            }
            Price = exchange.price;
            return this;
        }

    }
}
