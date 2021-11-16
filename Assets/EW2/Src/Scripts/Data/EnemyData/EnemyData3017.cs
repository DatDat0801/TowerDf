using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif

namespace EW2
{
    public class EnemyData3017 : EnemyData
    {
        public EnemyData3017Skill1[] allSkill1;
        public EnemyData3017Skill2[] allSkill2;
        
        public EnemyData3017Skill1 GetSkill1(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill1[level - 1];
        }
        
        public EnemyData3017Skill2 GetSkill2(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill2[level - 1];
        }
        
        [System.Serializable]
        public class EnemyData3017Skill1
        {
            public float secondApply;
            public float percentChanceApply;
            public float cooldown;
            public int waveTrigger;

        }
        
         
        [System.Serializable]
        public class EnemyData3017Skill2
        {
           
            public int waveTrigger;

        }
    }

#if UNITY_EDITOR
    public class Enemy3017Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3017";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3017";

                    // get asset file
                    string nameAsset = nameof(EnemyData3017) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3017 gm = AssetDatabase.LoadAssetAtPath<EnemyData3017>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3017>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    gm.allSkill1 = EnemyData.LoadAsset<EnemyData3017.EnemyData3017Skill1>(csvFormat, enemyId, 1);
                    gm.allSkill2 = EnemyData.LoadAsset<EnemyData3017.EnemyData3017Skill2>(csvFormat, enemyId, 2);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}