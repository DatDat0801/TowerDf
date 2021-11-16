
using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class ShopStaminaDataBase
    {
        public int moneyTypeToBuy;
        public int costExchange;
        public int valueExchangeByMoney;
        public long timeCountdown;
        public int valueExchangeByAds;
        public int adsCount;
        public int increaseRate;
    }

    public class ShopStaminaData : ScriptableObject
    {
        public ShopStaminaDataBase[] shopStaminaDataBases;
    }
#if UNITY_EDITOR

    public class ShopStaminaPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/shop_stamina.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(ShopStaminaData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile1 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;

                    ShopStaminaData gm = AssetDatabase.LoadAssetAtPath<ShopStaminaData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<ShopStaminaData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopStaminaDataBases = CsvReader.Deserialize<ShopStaminaDataBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    Debug.Log("Reimport Asset: " + str);
                    //shop_1
                    ShopStaminaData gm1 = AssetDatabase.LoadAssetAtPath<ShopStaminaData>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<ShopStaminaData>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    gm1.shopStaminaDataBases = CsvReader.Deserialize<ShopStaminaDataBase>(data.text);

                    EditorUtility.SetDirty(gm1);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
