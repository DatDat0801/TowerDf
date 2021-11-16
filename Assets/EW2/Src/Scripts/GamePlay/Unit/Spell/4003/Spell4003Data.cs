using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    public class Spell4003Data : SpellData
    {
        public Spell4003PassiveData[] passive;

        public override List<string> GetDescStatSkillActive(int level)
        {
            List<string> result = new List<string>();
            if (level >= spellStats.Length)
                level = spellStats.Length;
            var data = spellStats[level - 1];

            result.Add($"{data.damage.ToString()}");
            return result;
        }

        public override List<string> GetDescStatSkillPassive(int level)
        {
            List<string> result = new List<string>();
            if (level >= passive.Length)
                level = passive.Length;
            var data = passive[level - 1];

            result.Add($"{(data.explosionRatio * 100).ToString()}");
            result.Add($"{data.damage.ToString()}");
            return result;
        }
    }

    [Serializable]
    public class Spell4003PassiveData
    {
        public int level;
        public float explosionRatio;
        public float damage;
        public float radius;
    }
#if UNITY_EDITOR
    public class Spell4003Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/Spell4003";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get spell id
                    //var spellId = "4003";
                    var spellIds = new List<string>() {"4003","40031","40032"};

                    foreach (var spellId in spellIds)
                    {
                        string nameAsset = $"Spell{spellId}Data" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                        Spell4003Data gm = AssetDatabase.LoadAssetAtPath<Spell4003Data>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<Spell4003Data>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        string nameBaseCsv = $"{csvFormat}/spell_{spellId}_base.csv";

                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                        gm.spellStats = CsvReader.Deserialize<SpellStatBase>(data.text);

                        // get spell passive 1 file
                        gm.passive = SpellData.LoadAsset<Spell4003PassiveData>(csvFormat, spellId, "1");
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