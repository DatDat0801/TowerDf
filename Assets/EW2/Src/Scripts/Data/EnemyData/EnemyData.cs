using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif

namespace EW2
{
    [System.Serializable]
    public class EnemyStatBase
    {
        public int id;
        public int level;
        public string name;
        public float health;
        public float moveSpeed;
        public float attackSpeed;
        public float armor;
        public float resistance;
        public float damage;
        public float detectMeleeAttack;
        public float detectRangeAttack;
        public float critChance;
        public float critDamage;
        public float damageArea;
        public int life;
        public DamageType damageType;
        public MoveType moveType;
        public PriorityTargetType[] priorityTarget;
        public TraitEnemyType traitType;
        public int weight;
    }

    public class EnemyData : ScriptableObject
    {
        public EnemyStatBase[] allStatsLevel;

        public EnemyStatBase GetStats(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allStatsLevel[level - 1];
        }

#if UNITY_EDITOR
        public static T[] LoadAsset<T>(string csvFormat, string enemyId, int skillId)
        {
            string skillCsv = $"{csvFormat}/enemy_{enemyId}_skill_{skillId}.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(skillCsv);
            
            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }
}