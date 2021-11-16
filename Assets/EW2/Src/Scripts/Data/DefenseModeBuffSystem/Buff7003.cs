using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class SpellEnhancement
    {
        public int spellId;
        public float increaseDamageRatio;
    }

    public class Buff7003 : BuffBase
    {
        public SpellEnhancement[] spellEnhancements;

        public override List<string> GetDescStatSkillActive()
        {
            return new List<string>() { (spellEnhancements[0].increaseDamageRatio * 100).ToString() };
        }
    }
#if UNITY_EDITOR
    public class SpellEnhancementPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/DefenseModeBuff";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    var buffId = "7003";

                    // get asset file
                    string nameAsset = $"Buff{buffId}" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    Buff7003 gm = AssetDatabase.LoadAssetAtPath<Buff7003>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Buff7003>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/buff_{buffId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.spellEnhancements = CsvReader.Deserialize<SpellEnhancement>(data.text);
                    gm.buffId = buffId;

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
