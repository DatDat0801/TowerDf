

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class CriticalEnhancement
    {
        public float increaseCritical;
    }

    public class Buff7005 : BuffBase
    {
        public CriticalEnhancement[] criticalEnhancements;
        public CriticalEnhancement GetFirst()
        {
            return this.criticalEnhancements[0];
        }
        public override List<string> GetDescStatSkillActive()
        {
            return new List<string>() { criticalEnhancements[0].increaseCritical.ToString() };
        }
    }
#if UNITY_EDITOR
    public class CriticalEnhancementPostprocessor : AssetPostprocessor
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
                    var buffId = "7005";

                    // get asset file
                    string nameAsset = $"Buff{buffId}" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    Buff7005 gm = AssetDatabase.LoadAssetAtPath<Buff7005>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Buff7005>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/buff_{buffId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.criticalEnhancements = CsvReader.Deserialize<CriticalEnhancement>(data.text);
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
