using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class SpellFlashSaleData : ShopData
    {
        public PackCondition[] packConditions;
    }
#if UNITY_EDITOR

    public class SpellFlashSaleBundlePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/spell_flash_sale.csv";
            string csvConditon = "Assets/EW2/CSV/Shops/spell_flash_sale_condition.csv";
            string csvFormat1 = "Assets/EW2/CSV/Shops_1/spell_flash_sale.csv";
            string csvConditon1 = "Assets/EW2/CSV/Shops_1/spell_flash_sale_condition.csv";

            foreach (string str in importedAssets)
            {
                if ((str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 ||
                     str.IndexOf(csvConditon, StringComparison.Ordinal) != -1) &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(SpellFlashSaleData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile1 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;

                    SpellFlashSaleData gm = AssetDatabase.LoadAssetAtPath<SpellFlashSaleData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<SpellFlashSaleData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    TextAsset dataConditon = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.packConditions = CsvReader.Deserialize<PackCondition>(dataConditon.text);

                    EditorUtility.SetDirty(gm);

                    // data test 1
                    SpellFlashSaleData gm1 = AssetDatabase.LoadAssetAtPath<SpellFlashSaleData>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<SpellFlashSaleData>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    TextAsset data1 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat1);
                    TextAsset dataConditon1 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvConditon1);

                    gm1.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data1.text);
                    gm1.packConditions = CsvReader.Deserialize<PackCondition>(dataConditon1.text);

                    EditorUtility.SetDirty(gm1);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}