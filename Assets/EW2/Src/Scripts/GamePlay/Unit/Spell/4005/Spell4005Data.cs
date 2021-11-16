using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    public class Spell4005Data : SpellData
    {
        public Spell4005SkillData[] healsData;

        public override List<string> GetDescStatSkillActive(int level)
        {
            List<string> result = new List<string>();
            if (level >= spellStats.Length)
                level = spellStats.Length;
            var data = spellStats[level - 1];
            var data1 = healsData[level - 1];

            result.Add($"{data1.healActiveHp.ToString()}");
            result.Add($"{data.duration.ToString()}");
            return result;
        }

        public override List<string> GetDescStatSkillPassive(int level)
        {
            List<string> result = new List<string>();
            if (level >= healsData.Length)
                level = healsData.Length;
            var data = healsData[level - 1];

            result.Add($"{data.healPassiveHp.ToString()}");
            return result;
        }
    }

    [Serializable]
    public class Spell4005SkillData
    {
        public float level;
        public float healPassiveHp;
        public float healActiveHp;
    }

#if UNITY_EDITOR
    public class Spell4005Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/Spell4005";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get spell id
                    var spellIds = new List<string>() {"4005","40051","40052"};
                    foreach (var spellId in spellIds)
                    {
                        // get asset file
                        string nameAsset = $"Spell{spellId}Data" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                        Spell4005Data gm = AssetDatabase.LoadAssetAtPath<Spell4005Data>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<Spell4005Data>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        // get spell base file
                        string nameBaseCsv = $"{csvFormat}/spell_{spellId}_base.csv";

                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                        gm.spellStats = CsvReader.Deserialize<SpellStatBase>(data.text);

                        // get spell passive 1 file
                        gm.healsData = SpellData.LoadAsset<Spell4005SkillData>(csvFormat, spellId, "1");

                        EditorUtility.SetDirty(gm);
                    }

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}