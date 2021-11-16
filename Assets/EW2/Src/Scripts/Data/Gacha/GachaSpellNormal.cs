using System;
using EW2.Spell;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class GachaSpellNormal : GachaSpellData
    {
    }

#if UNITY_EDITOR

    public class GachaSpellNormalDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Gacha/";
            string csvDataBase = $"{csvFormat}gacha_spell_normal.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GachaSpellNormal) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Gacha/" + nameAsset;
                    GachaSpellNormal gm = AssetDatabase.LoadAssetAtPath<GachaSpellNormal>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GachaSpellNormal>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.gachaSpellDataBases = CsvReader.Deserialize<GachaSpellDataBase>(data.text);

                    gm.gachaSpellRateDatas =
                        GachaSpellNormal.LoadAssetRateData<GachaSpellRateData>(csvFormat, "gacha_spell_normal_rate");

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}