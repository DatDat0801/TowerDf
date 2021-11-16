using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    [Serializable]
    public class SpellStatBase
    {
        public int spellId;
        public int level;
        //public int rarity;
        public float damage;
        public int hp;
        public DamageType damageType;
        public float range;
        public MoveType targetType;
        public SpellUseType useType;
        public float duration;
        public float cooldown;
    }

    public class SpellData : ScriptableObject
    {
        /// <summary>
        /// Each level has a stat base
        /// </summary>
        public SpellStatBase[] spellStats;

        public virtual List<string> GetDescStatSkillActive(int level)
        {
            return new List<string>();
        }
        public virtual List<string> GetDescStatSkillPassive(int level)
        {
            return new List<string>();
        }

#if UNITY_EDITOR
        public static T[] LoadAsset<T>(string csvFormat, string spellId, string skillId)
        {
            string skillCsv = $"{csvFormat}/spell_{spellId}_skill_{skillId}.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(skillCsv);

            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }
}