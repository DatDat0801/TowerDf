using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class GachaSpellCopyRate : ScriptableObject
    {
        public SpellCopyRate[] spellCopyRates;

        [Serializable]
        public class SpellCopyRate
        {
            public int idSpell;
            public int rate;
        }

        public int GetRateConvertCopy(int spellId)
        {
            foreach (var rate in spellCopyRates)
            {
                if (rate.idSpell == spellId)
                    return rate.rate;
            }

            return 0;
        }
    }

#if UNITY_EDITOR

    public class GachaSpellCopyRatePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Gacha/";
            string csvDataBase = $"{csvFormat}gacha_spell_copy_rate.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GachaSpellCopyRate) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Gacha/" + nameAsset;
                    GachaSpellCopyRate gm = AssetDatabase.LoadAssetAtPath<GachaSpellCopyRate>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GachaSpellCopyRate>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.spellCopyRates = CsvReader.Deserialize<GachaSpellCopyRate.SpellCopyRate>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}