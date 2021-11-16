using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class EnemyData3020 : EnemyData
    {
        public EnemyData3020TriggerChangePhase[] triggerChangePhases;
        public EnemyData3020Skill1[] data3020Skill1;
        public EnemyData3020Skill2[] data3020Skill2;
        public EnemyData3020Skill3[] data3020Skill3;
        public EnemyData3020Skill4[] data3020Skill4;
        public EnemyData3020DamageRate[] data3020DamageRate;

        public EnemyData3020Skill1 GetSkill1(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return data3020Skill1[level - 1];
        }

        public EnemyData3020Skill2 GetSkill2(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return data3020Skill2[level - 1];
        }

        public EnemyData3020Skill3 GetSkill3(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return data3020Skill3[level - 1];
        }

        public EnemyData3020Skill4 GetSkill4(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return data3020Skill4[level - 1];
        }

        public EnemyData3020DamageRate GetDamageRate(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return data3020DamageRate[level - 1];
        }

        public EnemyData3020TriggerChangePhase GetTriggerEndPhase(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return triggerChangePhases[level - 1];
        }

        [Serializable]
        public class EnemyData3020Skill1
        {
            public float damage;
            public DamageType damageType;
            public int numberTarget;
            public float ratioPoisonPerSecond;
            public float timeLifePoison;
            public float intervalTimePoison;
            public float cooldown;
        }

        [Serializable]
        public class EnemyData3020Skill2
        {
            public float damage;
            public DamageType damageType;
            public int numberTarget;
            public float cooldown;
        }

        [Serializable]
        public class EnemyData3020Skill3
        {
            public int numberSummon;
            public int[] enemySummon;
            public float cooldown;
        }

        [Serializable]
        public class EnemyData3020Skill4
        {
            public float damage;
            public DamageType damageType;
            public float rangeDetect;
            public float timeStun;
            public float cooldown;
        }

        [Serializable]
        public class EnemyData3020TriggerChangePhase
        {
            public float percentHp;
            public int waveTrigger;
        }

        [Serializable]
        public class EnemyData3020DamageRate
        {
            public float damageRateMeleePhase2;
            public float damageRateMeleePhase3;
            public float damageRateRangePhase3;
        }
    }

#if UNITY_EDITOR
    public class Enemy3020Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3020";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3020";

                    // get asset file
                    string nameAsset = nameof(EnemyData3020) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3020 gm = AssetDatabase.LoadAssetAtPath<EnemyData3020>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3020>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";
                    var nameTriggerCsv = $"{csvFormat}/enemy_{enemyId}_trigger_change_phase.csv";
                    var nameDamageRateCsv = $"{csvFormat}/enemy_{enemyId}_damage_rate.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    gm.data3020Skill1 = EnemyData.LoadAsset<EnemyData3020.EnemyData3020Skill1>(csvFormat, enemyId, 1);
                    gm.data3020Skill2 = EnemyData.LoadAsset<EnemyData3020.EnemyData3020Skill2>(csvFormat, enemyId, 2);
                    gm.data3020Skill3 = EnemyData.LoadAsset<EnemyData3020.EnemyData3020Skill3>(csvFormat, enemyId, 3);
                    gm.data3020Skill4 = EnemyData.LoadAsset<EnemyData3020.EnemyData3020Skill4>(csvFormat, enemyId, 4);

                    TextAsset dataTrigger = AssetDatabase.LoadAssetAtPath<TextAsset>(nameTriggerCsv);
                    gm.triggerChangePhases =
                        CsvReader.Deserialize<EnemyData3020.EnemyData3020TriggerChangePhase>(dataTrigger.text);

                    TextAsset dataDamageRate = AssetDatabase.LoadAssetAtPath<TextAsset>(nameDamageRateCsv);
                    gm.data3020DamageRate =
                        CsvReader.Deserialize<EnemyData3020.EnemyData3020DamageRate>(dataDamageRate.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}