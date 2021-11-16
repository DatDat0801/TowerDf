using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class AttackEnhancement
    {
        public float damage;
    }

    public class Buff7001 : BuffBase
    {
        public AttackEnhancement[] attackEnhancements;

        public AttackEnhancement GetFirst()
        {
            return this.attackEnhancements[0];
        }

        public override List<string> GetDescStatSkillActive()
        {
            return new List<string>() { attackEnhancements[0].damage.ToString() };
        }
    }
#if UNITY_EDITOR
    public class AttackEnhancementPostprocessor : AssetPostprocessor
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
                    var buffId = "7001";

                    // get asset file
                    string nameAsset = $"Buff{buffId}" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    Buff7001 gm = AssetDatabase.LoadAssetAtPath<Buff7001>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Buff7001>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/buff_{buffId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.attackEnhancements = CsvReader.Deserialize<AttackEnhancement>(data.text);
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
