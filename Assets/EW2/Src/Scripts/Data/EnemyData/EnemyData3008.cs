using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class EnemyData3008: EnemyData
    {
     public EnemyData3008Skill1[] allSkill1;

        
        public EnemyData3008Skill1 GetSkill1ByLevel(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill1[level - 1];
        }
        
        /// <summary>
        /// Summon randomly
        /// </summary>
        [Serializable] 
        public class EnemyData3008Skill1
        {
            public int[] summonEnemyIds;
            public float[] chanceSummon;
            public int numberSummon;
            public int cooldown;
            
        }
    }
#if UNITY_EDITOR
    public class EnemyData3008Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3008";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3008";

                    // get asset file
                    string nameAsset = nameof(EnemyData3008) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3008 gm = AssetDatabase.LoadAssetAtPath<EnemyData3008>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3008>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    
                    gm.allSkill1 = EnemyData.LoadAsset<EnemyData3008.EnemyData3008Skill1>(csvFormat, enemyId, 1);


                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}