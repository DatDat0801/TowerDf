using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class RuneSetBonusDatabase : ScriptableObject
    {
        public RuneSetBonusStat[] setBonusStats;

        public RuneSetBonusStat GetDataRuneSet(int runeId, int runeSetType)
        {
            foreach (var setBonus in setBonusStats)
            {
                if (setBonus.runeId == runeId && setBonus.setQuantity == runeSetType)
                    return setBonus;
            }

            return null;
        }
    }
#if UNITY_EDITOR
    public class RuneSetBonusDatabasePostPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Runes/";
            string csvDataBase = $"{csvFormat}rune_set_bonus.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RuneSetBonusDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    RuneSetBonusDatabase gm = AssetDatabase.LoadAssetAtPath<RuneSetBonusDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RuneSetBonusDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.setBonusStats = CsvReader.Deserialize<RuneSetBonusStat>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}