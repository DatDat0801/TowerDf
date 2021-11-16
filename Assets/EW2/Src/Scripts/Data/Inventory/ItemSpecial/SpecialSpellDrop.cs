using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class SpecialSpellDrop : ScriptableObject
    {
        public DropInfo[] specialSpellDrops;
    }

#if UNITY_EDITOR

    public class SpecialSpellDropPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/ItemSpecials/spell_special_drop.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(SpecialSpellDrop) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    SpecialSpellDrop gm = AssetDatabase.LoadAssetAtPath<SpecialSpellDrop>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<SpecialSpellDrop>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.specialSpellDrops = CsvReader.Deserialize<DropInfo>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}