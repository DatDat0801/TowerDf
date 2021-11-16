using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [System.Serializable]
    public class ShopCrystalItemData
    {
        public int id;
        public int imgId;
        public float costExchange;
        public float costExchangePrevious;
        public int moneyTypeExchange;
        public float valueExchange;
        public float valuePrevious;
        public SaleType saleType;
    }

    public class ShopCrystalData : ScriptableObject
    {
        public ShopCrystalItemData[] shopCrystalItemDatas;
    }
#if UNITY_EDITOR

    public class ShopCrystalPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/shop_crystal.csv";
            string csvFormat1 = "Assets/EW2/CSV/Shops_1/shop_crystal.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(ShopCrystalData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile1 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;

                    ShopCrystalData gm = AssetDatabase.LoadAssetAtPath<ShopCrystalData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<ShopCrystalData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopCrystalItemDatas = CsvReader.Deserialize<ShopCrystalItemData>(data.text);

                    EditorUtility.SetDirty(gm);

                    // data test 1
                    ShopCrystalData gm1 = AssetDatabase.LoadAssetAtPath<ShopCrystalData>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<ShopCrystalData>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    TextAsset data1 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat1);

                    gm1.shopCrystalItemDatas = CsvReader.Deserialize<ShopCrystalItemData>(data1.text);

                    EditorUtility.SetDirty(gm1);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
