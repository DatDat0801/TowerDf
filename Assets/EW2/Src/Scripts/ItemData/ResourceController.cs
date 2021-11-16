using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class ResourceController
    {
        public ResourceController()
        {
            this.ResourceDict = new Dictionary<ResourceType, ResourceBase>();
        }

        public Dictionary<ResourceType, ResourceBase> ResourceDict { get; }

        public void Add(ResourceType resourceType, params object[] args)
        {
            var resourceBase = GetResourceBase(resourceType);

            resourceBase.Add(args);
        }

        public void Sub(ResourceType resourceType, params object[] args)
        {
            var resourceBase = GetResourceBase(resourceType);

            resourceBase.Sub(args);
        }

        public void UpdateInventoryData(int inventoryType, ItemInventoryBase itemInventory)
        {
            var resourceBase = (Inventory) GetResourceBase(ResourceType.Inventory);
            resourceBase.UpdateInventory(inventoryType, itemInventory);
        }

        public void Clear(ResourceType resourceType)
        {
            var resourceBase = GetResourceBase(resourceType);

            resourceBase.Clear();
        }

        public long GetMoneyInGame(int moneyType)
        {
            var resourceBase = GetResourceBase(ResourceType.MoneyInGame);

            return (long) resourceBase.Get(moneyType);
        }

        public long GetMoney(int moneyType)
        {
            var resourceBase = GetResourceBase(ResourceType.Money);

            return (long) resourceBase.Get(moneyType);
        }

        public ItemInventoryBase GetItemInventory(int itemType, int itemId)
        {
            var resourceBase = GetResourceBase(ResourceType.Inventory);

            return (ItemInventoryBase) resourceBase.Get(itemType, itemId);
        }

        public List<ItemInventoryBase> GetListSpell()
        {
            var resourceBase = (Inventory) GetResourceBase(ResourceType.Inventory);
            if (resourceBase != null)
            {
                return resourceBase.GetAllInventoryByType(InventoryType.Spell);
            }

            return new List<ItemInventoryBase>();
        }

        public List<ItemInventoryBase> GetListRune()
        {
            var resourceBase = (Inventory) GetResourceBase(ResourceType.Inventory);
            if (resourceBase != null)
            {
                return resourceBase.GetAllInventoryByType(InventoryType.Rune);
            }

            return new List<ItemInventoryBase>();
        }

        
        private ResourceBase GetResourceBase(ResourceType resourceType)
        {
            // Debug.Assert(resourceType == ResourceType.Money || resourceType == ResourceType.MoneyInGame,
            //     "ResourceType is invalid: " + resourceType.ToString());

            ResourceBase resourceBase;

            if (ResourceDict.ContainsKey(resourceType) == false)
            {
                resourceBase = Create(resourceType);

                ResourceDict.Add(resourceType, resourceBase);
            }

            resourceBase = ResourceDict[resourceType];

            return resourceBase;
        }

        private ResourceBase Create(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Money:
                case ResourceType.MoneyInGame:
                    return new Money(type);
                case ResourceType.Inventory:
                    return new Inventory(type);
                default:
                    return null;
            }
        }
    }
}