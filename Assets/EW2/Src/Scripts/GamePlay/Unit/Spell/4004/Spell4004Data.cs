using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    public class Spell4004Data : SpellData
    {
        public Warrior4004Stat[] warriorStats;
        public Spell4004PassiveData[] passiveData;

        public override List<string> GetDescStatSkillActive(int level)
        {
            List<string> result = new List<string>();
            if (level >= spellStats.Length)
                level = spellStats.Length;
            var data = spellStats[level - 1];

            if (level >= warriorStats.Length)
                level = warriorStats.Length;
            var dataWarrior = warriorStats[level - 1];
            
            result.Add($"{data.damage}");
            result.Add($"{dataWarrior.hp}");
            result.Add($"{dataWarrior.damage}");
            return result;
        }

        public override List<string> GetDescStatSkillPassive(int level)
        {
            List<string> result = new List<string>();
            if (level >= passiveData.Length)
                level = passiveData.Length;
            var data = passiveData[level - 1];

            result.Add($"{(data.armorRatio * 100).ToString()}");
            result.Add($"{(data.magicResistantRatio * 100).ToString()}");
            return result;
        }
    }

    [Serializable]
    public class Spell4004PassiveData
    {
        public int level;
        public float virtualHp;
        public float armorRatio;
        public float magicResistantRatio;
    }

    [Serializable]
    public class Warrior4004Stat
    {
        public float damage;

        public DamageType damageType;

        //public float range;
        public float hp;

        public float moveSpeed;
        public float attackSpeed;
        public float armor;
        public float resistance;
        public float critChance;
        public float critDamage;
        public int blockEnemy;
        public MoveType searchTarget;

        public float detectMeleeAttack;

        //public float detectRangeAttack;
        public float detectBlock;
        //public float stunInSecond;
    }

#if UNITY_EDITOR
    public class Spell4004Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/Spell4004";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    var spellIds = new List<string>() {"4004", "40041"};
                    foreach (var spellId in spellIds)
                    {
                        // get asset file
                        string nameAsset = $"Spell{spellId}Data" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                        Spell4004Data gm = AssetDatabase.LoadAssetAtPath<Spell4004Data>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<Spell4004Data>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        // get spell base file
                        string nameBaseCsv = $"{csvFormat}/spell_{spellId}_base.csv";

                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                        gm.spellStats = CsvReader.Deserialize<SpellStatBase>(data.text);
                        //gm.warriorStats = CsvReader.Deserialize<Warrior4004Stat>(data.text);
                        //meteor
                        string meteorCsv = $"{csvFormat}/spell_{spellId}_warrior.csv";
                        TextAsset meteorData = AssetDatabase.LoadAssetAtPath<TextAsset>(meteorCsv);
                        gm.warriorStats = CsvReader.Deserialize<Warrior4004Stat>(meteorData.text);

                        // get spell passive 1 file
                        gm.passiveData = SpellData.LoadAsset<Spell4004PassiveData>(csvFormat, spellId, "1");

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