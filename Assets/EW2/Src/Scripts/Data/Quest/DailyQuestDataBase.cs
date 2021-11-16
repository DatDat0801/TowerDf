using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class DailyQuestDataBase : ScriptableObject
    {
        public DailyQuestBase[] questDaily;

        public DailyQuestBase GetQuestDataById(int questId)
        {
            foreach (var quest in questDaily)
            {
                if (quest.questId == questId)
                    return quest;
            }

            return null;
        }

        public List<DailyQuestBase> GetListDailyQuest()
        {
            return questDaily.ToList();
        }
    }

#if UNITY_EDITOR

    public class DailyQuestPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Quest/daily_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(DailyQuestDataBase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Quest/" + nameAsset;
                    DailyQuestDataBase gm = AssetDatabase.LoadAssetAtPath<DailyQuestDataBase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DailyQuestDataBase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.questDaily = CsvReader.Deserialize<DailyQuestBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}