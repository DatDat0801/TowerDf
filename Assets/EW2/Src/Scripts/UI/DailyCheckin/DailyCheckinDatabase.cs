using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.DailyCheckin
{
    public class DailyCheckinDatabase : ScriptableObject
    {
        public DailyCheckinRewardItem[] rewardItems;
    }
    [Serializable]
    public class DailyCheckinRewardItem
    {
        public int day;
        public Reward reward;
    }
    #if UNITY_EDITOR

    public class DailyCheckinDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/DailyCheckin/";
            string csvDataBase = $"{csvFormat}daily_reward.csv";
            
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(DailyCheckinDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DailyCheckin/" + nameAsset;
                    DailyCheckinDatabase gm = AssetDatabase.LoadAssetAtPath<DailyCheckinDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DailyCheckinDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    var dailyCheckinRewardItems = CsvReader.Deserialize<DailyCheckinRewardItem>(data.text);

                    gm.rewardItems = dailyCheckinRewardItems;

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}