﻿using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class EnemyData3014 : EnemyData
    {
        
    }
    
#if UNITY_EDITOR
    public class Enemy3014Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Enemies/Enemy_3014";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var enemyId = "3014";

                    // get asset file
                    string nameAsset = nameof(EnemyData3014) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    EnemyData3014 gm = AssetDatabase.LoadAssetAtPath<EnemyData3014>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<EnemyData3014>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/enemy_{enemyId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}