

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class CooldownEnhancement
    {
        public float decreaseCooldown;
    }

    public class Buff7004 : BuffBase
    {
        public CooldownEnhancement[] cooldownEnhancements;
        public CooldownEnhancement GetFirst()
        {
            return this.cooldownEnhancements[0];
        }

        public override List<string> GetDescStatSkillActive()
        {
            return new List<string>() { (cooldownEnhancements[0].decreaseCooldown * 100).ToString() };
        }
    }
#if UNITY_EDITOR
    public class CooldownEnhancementPostprocessor : AssetPostprocessor
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
                    var buffId = "7004";

                    // get asset file
                    string nameAsset = $"Buff{buffId}" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    Buff7004 gm = AssetDatabase.LoadAssetAtPath<Buff7004>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Buff7004>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/buff_{buffId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.cooldownEnhancements = CsvReader.Deserialize<CooldownEnhancement>(data.text);
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
