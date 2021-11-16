using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class GachaSpellPremium : GachaSpellData
    {
    }

#if UNITY_EDITOR

    public class GachaSpellPremiumDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            const string csvFormat = "Assets/EW2/CSV/Gacha/";
            string csvDataBase = $"{csvFormat}gacha_spell_premium.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GachaSpellPremium) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Gacha/" + nameAsset;
                    GachaSpellPremium gm = AssetDatabase.LoadAssetAtPath<GachaSpellPremium>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GachaSpellPremium>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.gachaSpellDataBases = CsvReader.Deserialize<GachaSpellDataBase>(data.text);

                    gm.gachaSpellRateDatas =
                        GachaSpellData.LoadAssetRateData<GachaSpellRateData>(csvFormat, "gacha_spell_premium_rate");

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}