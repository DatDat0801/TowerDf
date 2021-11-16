using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class NewHeroEventBundle: ScriptableObject
    {
        public ShopLitmitedItemData[] shopLitmitItemDatas;
    }
    
#if UNITY_EDITOR

    public class NewHeroEventBundlePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/NewHero/new_hero_bundle.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(NewHeroEventBundle) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    NewHeroEventBundle gm = AssetDatabase.LoadAssetAtPath<NewHeroEventBundle>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<NewHeroEventBundle>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.shopLitmitItemDatas = CsvReader.Deserialize<ShopLitmitedItemData>(data.text);
                    
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}