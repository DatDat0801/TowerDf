using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class DefenseEnhancement
    {
        public float armor;
        public float magicResistance;
    }

    public class Buff7002 : BuffBase
    {
        public DefenseEnhancement[] defenseEnhancements;
        public DefenseEnhancement GetFirst()
        {
            return this.defenseEnhancements[0];
        }
        public override List<string> GetDescStatSkillActive()
        {
            return new List<string>() { defenseEnhancements[0].armor.ToString(), defenseEnhancements[0].magicResistance.ToString() };
        }
    }
#if UNITY_EDITOR
    public class DefenseEnhancementPostprocessor : AssetPostprocessor
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
                    var buffId = "7002";

                    // get asset file
                    string nameAsset = $"Buff{buffId}" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    Buff7002 gm = AssetDatabase.LoadAssetAtPath<Buff7002>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Buff7002>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/buff_{buffId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.defenseEnhancements = CsvReader.Deserialize<DefenseEnhancement>(data.text);
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
