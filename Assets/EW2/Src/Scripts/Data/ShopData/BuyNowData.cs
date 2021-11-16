using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class BuyNowData : ShopData
    {
        public PackCondition[] packConditions;

        [Serializable]
        public class PackCondition
        {
            public int mapUnlock;
            public int duration;
            public float percentProfit;
        }
    }

#if UNITY_EDITOR

    public class BuyNowPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/buy_now.csv";
            string csvConditon = "Assets/EW2/CSV/Shops/buy_now_condition.csv";
            
            string csvFormat0 = "Assets/EW2/CSV/Shops_1/buy_now.csv";
            string csvConditon0 = "Assets/EW2/CSV/Shops_1/buy_now_condition.csv";
            

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(BuyNowData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile0 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;
                    
                    // data orgin
                    BuyNowData gm = AssetDatabase.LoadAssetAtPath<BuyNowData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<BuyNowData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    TextAsset dataConditon = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.packConditions = CsvReader.Deserialize<BuyNowData.PackCondition>(dataConditon.text);

                    EditorUtility.SetDirty(gm);
                    
                    // data test 1
                    BuyNowData gm1 = AssetDatabase.LoadAssetAtPath<BuyNowData>(assetFile0);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<BuyNowData>();
                        AssetDatabase.CreateAsset(gm1, assetFile0);
                    }

                    TextAsset data0 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat0);
                    TextAsset dataConditon0 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon0);

                    gm1.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data0.text);
                    gm1.packConditions = CsvReader.Deserialize<BuyNowData.PackCondition>(dataConditon0.text);

                    EditorUtility.SetDirty(gm1);
                    
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}