using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    public class Spell4001Data : SpellData
    {
        public Spell4001PassiveData[] passive;
        public Spell4001ActiveData[] active;
        
        public override List<string> GetDescStatSkillActive(int level)
        {
            List<string> result = new List<string>();
            if (level >= spellStats.Length)
                level = spellStats.Length;
            var data = spellStats[level - 1];
            var passiveData = this.active[level - 1];
            result.Add($"{data.duration.ToString()}");
            result.Add($"{passiveData.coolDuration.ToString()}");
            return result;
        }
        public override List<string> GetDescStatSkillPassive(int level)
        {
            List<string> result = new List<string>();
            if (level >= passive.Length)
                level = passive.Length;
            var data = passive[level - 1];

            result.Add($"{(data.freezeRatio * 100).ToString()}");

            return result;
        }
    }
    [Serializable]
    public class Spell4001PassiveData
    {
        public int level;
        public float freezeRatio;
        public float slowDownPercent;
        public float duration;
    }
    [Serializable]
    public class Spell4001ActiveData
    {
        public float coolDuration;
        public float slowDownPercent;
    }
#if UNITY_EDITOR
    public class Spell4001Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/Spell4001";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get spell id
                    //var spellId = "4003";
                    var spellIds = new List<string>() {"4001","40011","40012"};

                    foreach (var spellId in spellIds)
                    {
                        string nameAsset = $"Spell{spellId}Data" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                        Spell4001Data gm = AssetDatabase.LoadAssetAtPath<Spell4001Data>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<Spell4001Data>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        string nameBaseCsv = $"{csvFormat}/spell_{spellId}_base.csv";

                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                        gm.spellStats = CsvReader.Deserialize<SpellStatBase>(data.text);
                        gm.active = CsvReader.Deserialize<Spell4001ActiveData>(data.text);

                        // get spell passive 1 file
                        gm.passive = SpellData.LoadAsset<Spell4001PassiveData>(csvFormat, spellId, "1");
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