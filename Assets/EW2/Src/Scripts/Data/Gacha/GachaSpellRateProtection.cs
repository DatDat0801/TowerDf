using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class SpellRateProtection
    {
        public int rarity;
        public float rate;
    }

    public class GachaSpellRateProtection : ScriptableObject
    {
        public SpellRateProtection[] spellRateProtections;
    }

#if UNITY_EDITOR

    public class GachaSpellRateProtectionDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Gacha/";
            string csvDataBase = $"{csvFormat}gacha_spell_protection.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GachaSpellRateProtection) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Gacha/" + nameAsset;
                    GachaSpellRateProtection gm = AssetDatabase.LoadAssetAtPath<GachaSpellRateProtection>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GachaSpellRateProtection>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.spellRateProtections = CsvReader.Deserialize<SpellRateProtection>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}