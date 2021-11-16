using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class SpellPackageUnlockCondition : UnlockCondition
    {
    }
    public class SpellpackageData : ShopData
    {
        public SpellPackageUnlockCondition[] unlockConditions;
    }
#if UNITY_EDITOR

    public class SpellPackageShopPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/spell_package_shop.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(SpellpackageData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;
                    SpellpackageData gm = AssetDatabase.LoadAssetAtPath<SpellpackageData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<SpellpackageData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopItemDatas = CsvReader.Deserialize<ShopItemData>(data.text);
                    gm.unlockConditions = CsvReader.Deserialize<SpellPackageUnlockCondition>(data.text);
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
