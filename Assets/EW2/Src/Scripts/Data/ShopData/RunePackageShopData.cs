using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class UnlockCondition
    {
        public int mapUnlock;
        //public double duration;
        public int dayOpenGame;
    }
    public class RunePackageShopData: ShopData
    {
        public UnlockCondition[] unlockConditions;
    }
#if UNITY_EDITOR

    public class RunePackageShopPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/rune_package_shop.csv";
            string csvFormat1 = "Assets/EW2/CSV/Shops_1/rune_package_shop.csv";
            
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RunePackageShopData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    string assetFile1 = "Assets/EW2/Resources/CSV/Shops_1/" + nameAsset;
                    
                    RunePackageShopData gm = AssetDatabase.LoadAssetAtPath<RunePackageShopData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RunePackageShopData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.unlockConditions = CsvReader.Deserialize<UnlockCondition>(data.text);
                    EditorUtility.SetDirty(gm);
                    
                    // data test 1
                    RunePackageShopData gm1 = AssetDatabase.LoadAssetAtPath<RunePackageShopData>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<RunePackageShopData>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    TextAsset data1 = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat1);

                    gm1.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data1.text);
                    gm1.unlockConditions = CsvReader.Deserialize<UnlockCondition>(data1.text);
                    EditorUtility.SetDirty(gm1);
                    
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}