using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class AchievementDataBase : ScriptableObject
    {
        public QuestBase[] achievementDatas;

        public QuestBase GetQuestDataById(int questId)
        {
            foreach (var quest in achievementDatas)
            {
                if (quest.questId == questId)
                    return quest;
            }

            return null;
        }

        public List<QuestBase> GetListDailyQuest()
        {
            return achievementDatas.ToList();
        }
    }

#if UNITY_EDITOR

    public class AchievementQuestPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Quest/achievement_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(AchievementDataBase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Quest/" + nameAsset;
                    AchievementDataBase gm = AssetDatabase.LoadAssetAtPath<AchievementDataBase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<AchievementDataBase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.achievementDatas = CsvReader.Deserialize<QuestBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}