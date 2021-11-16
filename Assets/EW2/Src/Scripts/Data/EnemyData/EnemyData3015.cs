using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif

namespace EW2
{
    public class EnemyData3015 : EnemyData
    {
        public EnemyData3015Skill1[] allSkill1;
        public EnemyData3015Skill2[] allSkill2;
        
        public EnemyData3015Skill1 GetSkill1(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill1[level - 1];
        }
        
        public EnemyData3015Skill2 GetSkill2(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill2[level - 1];
        }
        
        [System.Serializable]
        public class EnemyData3015Skill1
        {
            public float secondApply;
            public float percentChanceApply;
            
        }
        
        [System.Serializable]
        public class EnemyData3015Skill2
        {
            public float secondApply;
            public float percentIncreaseAttackSpeed;
            public float percentIncreaseDamage;
            public float percentHp;
            public ModifierType modifierTypeAttackSpeed;
            public ModifierType modifierTypeDamage;
        }
    }
   

#if UNITY_EDITOR
    public class Enemy3015Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3015";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3015";

                    // get asset file
                    string nameAsset = nameof(EnemyData3015) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3015 gm = AssetDatabase.LoadAssetAtPath<EnemyData3015>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3015>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    
                    gm.allSkill1 = EnemyData.LoadAsset<EnemyData3015.EnemyData3015Skill1>(csvFormat, enemyId, 1);
                    gm.allSkill2 = EnemyData.LoadAsset<EnemyData3015.EnemyData3015Skill2>(csvFormat, enemyId, 2);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}