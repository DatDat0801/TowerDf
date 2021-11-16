using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    public class Spell4011Data : SpellData
    {
        //public int totemHp;
        public Spell4011PassiveData[] passiveData;

        public override List<string> GetDescStatSkillActive(int level)
        {
            List<string> result = new List<string>();
            if (level >= spellStats.Length)
                level = spellStats.Length;
            var data = spellStats[level - 1];

            result.Add($"{data.hp}");
            result.Add($"{data.duration}");
            result.Add($"{data.damage}");

            return result;
        }

        public override List<string> GetDescStatSkillPassive(int level)
        {
            List<string> result = new List<string>();
            if (level >= passiveData.Length)
                level = passiveData.Length;
            var data = passiveData[level - 1];

            result.Add($"{data.hitsToTerrify}");
            result.Add($"{data.duration}");
            return result;
        }
    }

    [Serializable]
    public class Spell4011PassiveData
    {
        public int level;
        public int hitsToTerrify;
        public float duration;
        public float radius;
    }
#if UNITY_EDITOR
    public class Spell4011Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/Spell4011";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    var spellIds = new List<string>() {"4011", "40111"};
                    foreach (var spellId in spellIds)
                    {
                        // get asset file
                        string nameAsset = $"Spell{spellId}Data" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                        Spell4011Data gm = AssetDatabase.LoadAssetAtPath<Spell4011Data>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<Spell4011Data>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        // get spell base file
                        string nameBaseCsv = $"{csvFormat}/spell_{spellId}_base.csv";

                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                        gm.spellStats = CsvReader.Deserialize<SpellStatBase>(data.text);

                        // get spell passive 1 file
                        gm.passiveData = SpellData.LoadAsset<Spell4011PassiveData>(csvFormat, spellId, "1");

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