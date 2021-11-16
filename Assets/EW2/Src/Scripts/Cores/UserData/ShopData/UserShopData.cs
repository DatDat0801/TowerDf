using System;
using System.Collections.Generic;

namespace EW2
{
    [Serializable]
    public class UserShopData
    {
        public List<string> listProductIdNonconsume = new List<string>();
        public int totalPackageBuyed;
        public float totalRevenue;

        public bool CheckPackNonconsumePurchased(string producttId)
        {
            return listProductIdNonconsume.Contains(producttId);
        }

        public void AddProductIdNonconsumePurchased(string producttId)
        {
            if (!listProductIdNonconsume.Contains(producttId))
                listProductIdNonconsume.Add(producttId);
        }
    }
}