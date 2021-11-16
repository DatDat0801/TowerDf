using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{

    [System.Serializable]
    public class TowerStatBase
    {
        public int level;
        public float damage;
        public DamageType damageType;
        public float attackSpeed;
        public float critChance;
        public float critDamage;
        public MoveType searchTarget;
        public PriorityTargetType[] priorityTarget;
        public float detectRangeAttack;

        public TowerStatBase Clone()
        {
            var towerStatBase = new TowerStatBase();

            towerStatBase.damage = damage;
            towerStatBase.attackSpeed = attackSpeed;
            towerStatBase.critChance = critChance;
            towerStatBase.critDamage = critDamage;
            towerStatBase.detectRangeAttack = detectRangeAttack;
            towerStatBase.searchTarget = searchTarget;

            return towerStatBase;
        }
    }

    public abstract class TowerData : ScriptableObject
    {
        public int id;

        public abstract BuildingInfoContent GetInfo(int level);
        public abstract TowerStatBase GetDataStatBaseByLevel(int level);
        public abstract TowerStatBase GetDataStatFinalByLevel(int level);

        public abstract string GetInfoSkill(int skillId, int level, string desc);
        
#if UNITY_EDITOR
        public static T[] LoadAsset<T>(string csvFormat, string towerId, int skillId)
        {
            string nameCsv = $"{csvFormat}/tower_{towerId}_skill_{skillId}.csv";

            TextAsset dataPassive = AssetDatabase.LoadAssetAtPath<TextAsset>(nameCsv);

            return CsvReader.Deserialize<T>(dataPassive.text);
        }
#endif
    }
}