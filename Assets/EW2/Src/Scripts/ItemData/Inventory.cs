using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Inventory : ResourceBase
    {
        public Inventory(ResourceType type) : base(type)
        {
            InventoryDict = new Dictionary<int, List<ItemInventoryBase>>();
        }

        public Dictionary<int, List<ItemInventoryBase>> InventoryDict { get; }

        public override void Add(params object[] args)
        {
            var itemInventory = (ItemInventoryBase) args[0];

            if (itemInventory is RuneItem)
            {
                itemInventory.ItemId = UserData.Instance.OtherUserData.totalRune;
                UserData.Instance.OtherUserData.totalRune++;
            }

            if (!InventoryDict.ContainsKey(itemInventory.InventoryType))
            {
                List<ItemInventoryBase> listItem = new List<ItemInventoryBase>();

                listItem.Add(itemInventory);

                InventoryDict.Add(itemInventory.InventoryType, listItem);
            }
            else
            {
                if (itemInventory is RuneItem)
                {
                    InventoryDict[itemInventory.InventoryType].Add(itemInventory);
                }
                else
                {
                    var isCheck = false;

                    foreach (var item in InventoryDict[itemInventory.InventoryType])
                    {
                        if (item.ItemId == itemInventory.ItemId)
                        {
                            item.Quantity += itemInventory.Quantity;
                            isCheck = true;
                            return;
                        }
                    }

                    if (!isCheck)
                    {
                        InventoryDict[itemInventory.InventoryType].Add(itemInventory);
                    }
                }
            }
        }

        public void UpdateInventory(int inventoryType, ItemInventoryBase itemInventory)
        {
            if (InventoryDict.ContainsKey(inventoryType))
            {
                for (int i = 0; i < InventoryDict[inventoryType].Count; i++)
                {
                    if (InventoryDict[inventoryType][i].ItemId == itemInventory.ItemId)
                    {
                        InventoryDict[inventoryType][i] = itemInventory;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"item id {inventoryType} is not exist");
            }
        }

        public override void Sub(params object[] args)
        {
            var itemInventory = (ItemInventoryBase) args[0];

            if (InventoryDict[itemInventory.InventoryType].Contains(itemInventory))
            {
                InventoryDict[itemInventory.InventoryType].Remove(itemInventory);
            }
        }

        public override object Get(params object[] args)
        {
            var itemType = (int) args[0];

            var itemId = (int) args[1];

            if (InventoryDict.ContainsKey(itemType))
                foreach (var item in InventoryDict[itemType])
                {
                    if (item.ItemId == itemId)
                        return item;
                }

            return null;
        }

        public List<ItemInventoryBase> GetAllInventoryByType(int itemType)
        {
            if (InventoryDict.ContainsKey(itemType))
                return InventoryDict[itemType];
            return new List<ItemInventoryBase>();
        }

        public override void Clear()
        {
        }
    }
}