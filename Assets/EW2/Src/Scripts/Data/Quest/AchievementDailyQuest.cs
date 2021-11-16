using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class AchievementDailyQuest : ScriptableObject
    {
        public AchievementDaily[] achievementDailyDatas;

        [Serializable]
        public class AchievementDaily
        {
            public int achievementId;
            public int pointRequire;
            public Reward[] rewards;
        }

        public List<AchievementDaily> GetAchievements()
        {
            return achievementDailyDatas.ToList();
        }
    }


#if UNITY_EDITOR

    public class AchievementDailyPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Quest/achievement_daily_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(AchievementDailyQuest) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Quest/" + nameAsset;
                    AchievementDailyQuest gm = AssetDatabase.LoadAssetAtPath<AchievementDailyQuest>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<AchievementDailyQuest>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.achievementDailyDatas = CsvReader.Deserialize<AchievementDailyQuest.AchievementDaily>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}