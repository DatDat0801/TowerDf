using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class FirstPurchaseRewardDatabase : ScriptableObject
    {
        public Reward[] rewards;
        
        
    }
#if UNITY_EDITOR

    public class DailyCheckinDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/FirstPurchase/";
            string csvDataBase = $"{csvFormat}first_purchase_reward.csv";
            
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(FirstPurchaseRewardDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/FirstPurchase/" + nameAsset;
                    FirstPurchaseRewardDatabase gm = AssetDatabase.LoadAssetAtPath<FirstPurchaseRewardDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<FirstPurchaseRewardDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    var firstPurchaseRewards = CsvReader.Deserialize<Reward>(data.text);

                    gm.rewards = firstPurchaseRewards;

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}