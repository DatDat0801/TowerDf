using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;

#endif

namespace EW2
{
    [System.Serializable]
    public class HeroStatBase
    {
        public int id;
        public float health;
        public float moveSpeed;
        public float attackSpeed;
        public float armor;
        public float resistance;
        public float damage;
        public float critChance;
        public float critDamage;
        public float hpRegeneration;
        public float timeRevive;
        public DamageType damageType;
        public int blockEnemy;
        public float timeTriggerRegeneration;
        public MoveType searchTarget;
        public PriorityTargetType[] priorityTarget;
        public float detectMeleeAttack;
        public float detectRangeAttack;
        public float detectBlock;
        public int level;
        public float maxExp;
        public HeroClasses[] heroClasses;
    }

    public abstract class HeroData : ScriptableObject
    {
        public HeroStatBase[] stats;

        public abstract Dictionary<int, List<float>> GetDescStatSkill(int skillId);

        public virtual (RPGStatType, RPGStatModifier) GetStatModifierBySkill(int skillId, int level)
        {
            return (RPGStatType.None, null);
        }

#if UNITY_EDITOR
        public static T[] LoadAsset<T>(string csvFormat, string heroId, int skillId)
        {
            string namePassiveCsv = $"{csvFormat}/hero_{heroId}_skill_{skillId}.csv";

            TextAsset dataPassive = AssetDatabase.LoadAssetAtPath<TextAsset>(namePassiveCsv);

            return CsvReader.Deserialize<T>(dataPassive.text);
        }
#endif
    }
}