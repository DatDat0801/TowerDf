using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class DropInfo
    {
        public float rarity2Rate;
        public float rarity3Rate;
        public float rarity4Rate;
    }

    public class SpecialRuneDrop : ScriptableObject
    {
        public DropInfo[] specialRuneDrops;
    }

#if UNITY_EDITOR

    public class SpecialRuneDropPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/ItemSpecials/rune_special_drop.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(SpecialRuneDrop) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    SpecialRuneDrop gm = AssetDatabase.LoadAssetAtPath<SpecialRuneDrop>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<SpecialRuneDrop>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.specialRuneDrops = CsvReader.Deserialize<DropInfo>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}