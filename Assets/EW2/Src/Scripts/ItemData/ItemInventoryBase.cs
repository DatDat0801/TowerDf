using System;
using Zitga.Observables;

namespace EW2
{
    [Serializable]
    public class ItemInventoryBase
    {
        public int ItemId { get; set; }

        public long Quantity { get; set; }

        public int InventoryType { get; set; }

        public int Rarity { get; set; }

        public int Level { get; set; }

        public ItemInventoryBase(int id, int quantity, int inventoryType)
        {
            ItemId = id;

            Quantity = quantity;

            InventoryType = inventoryType;

            Rarity = 0;

            Level = 1;
        }
    }
}