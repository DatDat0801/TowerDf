using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class EnemyData3016 : EnemyData
    {
        public EnemyData3016Skill1[] allSkill1;
        public EnemyData3016Skill2[] allSkill2;
        
        public EnemyData3016Skill1 GetSkill1ByLevel(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill1[level - 1];
        }
        
        public EnemyData3016Skill2 GetSkill2ByLevel(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill2[level - 1];
        }
        /// <summary>
        /// Summon randomly
        /// </summary>
        [Serializable] 
        public class EnemyData3016Skill1
        {
            public int[] summonEnemyIds;
            public float[] chanceSummon;
            public int numberSummon;
            public int cooldown;
            
        }
        /// <summary>
        /// Freeze randomly 2 tower in 2 unit radius, in 2 second, cooldown 10s
        /// </summary>
        [Serializable] 
        public class EnemyData3016Skill2
        {
            public int numberOfTower;
            public float radius;
            public float inSecond;
            public float cooldown;
        }
    }
#if UNITY_EDITOR
    public class EnemyData3016Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3016";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3016";

                    // get asset file
                    string nameAsset = nameof(EnemyData3016) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3016 gm = AssetDatabase.LoadAssetAtPath<EnemyData3016>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3016>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    
                    gm.allSkill1 = EnemyData.LoadAsset<EnemyData3016.EnemyData3016Skill1>(csvFormat, enemyId, 1);
                    gm.allSkill2 = EnemyData.LoadAsset<EnemyData3016.EnemyData3016Skill2>(csvFormat, enemyId, 2);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}