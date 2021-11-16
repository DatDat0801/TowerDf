using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Rune10DataBases : RuneDataBases
    {
    }

#if UNITY_EDITOR

    public class Rune10DataBasesPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Runes/";
            string csvDataBase = $"{csvFormat}rune_10_base.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Rune10DataBases) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    Rune10DataBases gm = AssetDatabase.LoadAssetAtPath<Rune10DataBases>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Rune10DataBases>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.runeDataBases = CsvReader.Deserialize<RuneDataBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}