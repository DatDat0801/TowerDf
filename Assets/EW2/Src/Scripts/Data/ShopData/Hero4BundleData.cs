using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Hero4BundleData : ShopData
    {
        public PackCondition[] packConditions;

        public int GetProfit()
        {
            var profit = this.shopItemDatas[0].pricePrevious / this.shopItemDatas[0].price;
            return (int)profit * 100;
        }
    }
#if UNITY_EDITOR

    public class Hero4BundlePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/hero_4_bundle.csv";
            string csvConditon = "Assets/EW2/CSV/Shops/hero_4_bundle_condition.csv";
            string csvFormat0 = "Assets/EW2/CSV/Shops_1/hero_4_bundle.csv";
            string csvConditon0 = "Assets/EW2/CSV/Shops_1/hero_4_bundle_condition.csv";
            
            foreach (string str in importedAssets)
            {
                if ((str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 || str.IndexOf(csvConditon, StringComparison.Ordinal) != -1) &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Hero4BundleData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile0 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;
                    
                    // data orgin
                    Hero4BundleData gm = AssetDatabase.LoadAssetAtPath<Hero4BundleData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Hero4BundleData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    TextAsset dataConditon = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.packConditions = CsvReader.Deserialize<PackCondition>(dataConditon.text);

                    EditorUtility.SetDirty(gm);
                    
                    // data test 1
                    Hero4BundleData gm1 = AssetDatabase.LoadAssetAtPath<Hero4BundleData>(assetFile0);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<Hero4BundleData>();
                        AssetDatabase.CreateAsset(gm1, assetFile0);
                    }

                    TextAsset data0 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat0);
                    TextAsset dataConditon0 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon0);

                    gm1.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data0.text);
                    gm1.packConditions = CsvReader.Deserialize<PackCondition>(dataConditon0.text);

                    EditorUtility.SetDirty(gm1);
                    
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}