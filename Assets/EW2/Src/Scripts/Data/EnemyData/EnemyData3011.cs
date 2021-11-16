using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif

namespace EW2
{
    public class EnemyData3011 : EnemyData
    {
        public EnemyData3011Skill1[] allSkill1;
        
        public EnemyData3011Skill1 GetSkill1(int level)
        {
            if (level < 1 || level > allStatsLevel.Length)
                level = 1;

            return allSkill1[level - 1];
        }
        
        [System.Serializable]
        public class EnemyData3011Skill1
        {
            public float hpLost;
            public float secondApply;
            
        }
    }
   

#if UNITY_EDITOR
    public class Enemy3011Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3011";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3011";

                    // get asset file
                    string nameAsset = nameof(EnemyData3011) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3011 gm = AssetDatabase.LoadAssetAtPath<EnemyData3011>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3011>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    
                    gm.allSkill1 = EnemyData.LoadAsset<EnemyData3011.EnemyData3011Skill1>(csvFormat, enemyId, 1);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}