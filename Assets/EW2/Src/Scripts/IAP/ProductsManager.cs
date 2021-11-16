using System;
using System.Collections.Generic;
using System.Globalization;
using EW2.Events;
using UnityEngine;

namespace EW2
{
    public static class ProductsManager
    {
        private static readonly Dictionary<string, ShopItemData> nonConsumableProducts =
            new Dictionary<string, ShopItemData>();

        private static readonly Dictionary<string, ShopItemData> consumableProducts =
            new Dictionary<string, ShopItemData>();

        private static readonly Dictionary<string, string> localPricesString = new Dictionary<string, string>();
        private static readonly Dictionary<string, decimal> localPrice = new Dictionary<string, decimal>();

        private static Dictionary<string, ShopItemData> Products { get; } = new Dictionary<string, ShopItemData>();

        private static readonly Dictionary<int, ShopItemData> heroShopProducts = new Dictionary<int, ShopItemData>();

        public static Dictionary<string, ShopItemData> ProductsConsumable => consumableProducts;
        public static Dictionary<string, ShopItemData> ProductsNonConsumable => nonConsumableProducts;
        public static Dictionary<int, ShopItemData> HeroShopProducts => heroShopProducts;


        public static void AddProductBundleShop()
        {
            var gemData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopGemData>();
            if (gemData != null)
            {
                foreach (var bundle in gemData.shopItemDatas)
                {
                    AddProducts(bundle, true);
                }
            }

            var heroData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopHeroData>();
            if (heroData != null)
            {
                foreach (var bundle in heroData.shopItemDatas)
                {
                    AddProducts(bundle, true);
                    heroShopProducts[bundle.rewards[0].id] = bundle;
                }
            }

            var starterPack = GameContainer.Instance.Get<ShopDataBase>().Get<StarterPackData>();
            if (starterPack != null)
            {
                AddProducts(starterPack.shopItemDatas[0], true);
            }

            var buyNowPackage = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();
            if (buyNowPackage != null)
            {
                AddProducts(buyNowPackage.shopItemDatas[0], true);
            }

            var runePackage = GameContainer.Instance.Get<ShopDataBase>().Get<RunePackageShopData>();
            if (runePackage != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    AddProducts(runePackage.shopItemDatas[i], true);
                }
            }

            var hero4Bundle = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            if (hero4Bundle != null)
            {
                AddProducts(hero4Bundle.shopItemDatas[0], true);
            }

            var hero4ResourceFlashSale = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4ResourceFlashSaleData>();
            if (hero4ResourceFlashSale != null)
            {
                AddProducts(hero4ResourceFlashSale.shopItemDatas[0], true);
            }

            var runeFlashSale = GameContainer.Instance.Get<ShopDataBase>().Get<RuneFlashSaleData>();
            if (runeFlashSale != null)
            {
                AddProducts(runeFlashSale.shopItemDatas[0], true);
            }

            var spellFlashSale = GameContainer.Instance.Get<ShopDataBase>().Get<SpellFlashSaleData>();
            if (spellFlashSale != null)
            {
                AddProducts(spellFlashSale.shopItemDatas[0], true);
            }

            var spellPackage = GameContainer.Instance.Get<ShopDataBase>().Get<SpellpackageData>();
            if (spellPackage != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    AddProducts(spellPackage.shopItemDatas[i], true);
                }
            }

            var newHeroEventBundle = GameContainer.Instance.Get<EventDatabase>().Get<NewHeroEventBundle>();
            if (newHeroEventBundle != null)
            {
                foreach (var bundle in newHeroEventBundle.shopLitmitItemDatas)
                {
                    var itemShopBundle = new ShopItemData();
                    itemShopBundle.ConvertDataToLimitedBundle(bundle);
                    AddProducts(itemShopBundle, true);
                }
            }
        }

        public static void AddProducts<T>(List<T> data) where T : ShopItemData
        {
            foreach (var item in data)
            {
                if (item == null) return;
                var isConsumable = item.consumable > 0;
                AddProducts(item, isConsumable);
            }
        }

        public static void AddProducts<T>(List<T> data, bool isConsumable) where T : ShopItemData
        {
            foreach (var item in data)
            {
                if (item == null) return;
                AddProducts(item, isConsumable);
            }
        }

        private static void AddProducts<T>(T item, bool isConsumable) where T : ShopItemData
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.productId))
            {
                if (isConsumable)
                {
                    consumableProducts[item.productId] = item;
                }
                else
                {
                    nonConsumableProducts[item.productId] = item;
                }

                if (Products.ContainsKey(item.productId))
                    Log.Error("[IAP] productId exists: " + item.productId);

                Products[item.productId] = item;
            }
        }

        public static ShopItemData GetItemShopById(string productId)
        {
            if (Products.ContainsKey(productId))
                return Products[productId];

            return null;
        }

        public static string GetPriceById(string productId)
        {
            try
            {
                var price = localPrice[productId];

                return price.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Can't get price of {0}: {1}", productId, e));
                return "";
            }
        }

        public static string GetLocalPriceStringById(string productId)
        {
            try
            {
                string price = "";

                if (localPricesString.ContainsKey(productId))
                    price = localPricesString[productId];

                if (price.Equals("$0.01"))
                {
                    price = string.Empty;
                }

                return price;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Can't get price of {0}: {1}", productId, e));
                return "";
            }
        }

        public static void AddPrice(string productId, decimal price)
        {
            if (!localPrice.ContainsKey(productId))
            {
                localPrice.Add(productId, price);
            }
        }

        public static void AddPriceString(string productId, string price)
        {
            if (!localPricesString.ContainsKey(productId))
            {
                localPricesString.Add(productId, price);
            }
        }

        public static void ClearProducts()
        {
            consumableProducts.Clear();
            nonConsumableProducts.Clear();
            Products.Clear();
        }

        public static bool CheckReceived(ShopItemData data)
        {
            var isReceived = false;

            // if (data == null)
            // {
            //     Debug.LogError("[IAP] Data null, isReceived = true");
            //     isReceived = true;
            //     return isReceived;
            // }
            //
            // if (data.consumable > 0)
            // {
            //     return isReceived;
            // }
            //
            // isReceived = UserData.Instance.UserShopData.CheckPackNonconsumePurchased(data.productId);

            return isReceived;
        }
    }
}