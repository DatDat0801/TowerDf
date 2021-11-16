using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Hero4ResourceFlashSaleData : ShopData
    {
        public PackCondition[] packConditions;
    }
#if UNITY_EDITOR

    public class Hero4ResourceFlashSalePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/hero_4_resource_flash_sale.csv";
            string csvConditon = "Assets/EW2/CSV/Shops/hero_4_resource_flash_sale_condition.csv";
            string csvFormat0 = "Assets/EW2/CSV/Shops_1/hero_4_resource_flash_sale.csv";
            string csvConditon0 = "Assets/EW2/CSV/Shops_1/hero_4_resource_flash_sale_condition.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Hero4ResourceFlashSaleData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile0 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;
                    
                    // data orgin
                    Hero4ResourceFlashSaleData gm = AssetDatabase.LoadAssetAtPath<Hero4ResourceFlashSaleData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Hero4ResourceFlashSaleData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    TextAsset dataConditon = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.packConditions = CsvReader.Deserialize<PackCondition>(dataConditon.text);

                    EditorUtility.SetDirty(gm);
                    
                    // data test 1
                    Hero4ResourceFlashSaleData gm1 = AssetDatabase.LoadAssetAtPath<Hero4ResourceFlashSaleData>(assetFile0);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<Hero4ResourceFlashSaleData>();
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