using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class ShopHeroData : ShopData
    {
    }

#if UNITY_EDITOR

    public class ShopHeroPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/shop_hero.csv";
            string csvFormat1 = "Assets/EW2/CSV/Shops_1/shop_hero.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(ShopHeroData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile1 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;

                    ShopHeroData gm = AssetDatabase.LoadAssetAtPath<ShopHeroData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<ShopHeroData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);

                    EditorUtility.SetDirty(gm);

                    // data test 1
                    ShopHeroData gm1 = AssetDatabase.LoadAssetAtPath<ShopHeroData>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<ShopHeroData>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    TextAsset data1 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat1);

                    gm1.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data1.text);

                    EditorUtility.SetDirty(gm1);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}