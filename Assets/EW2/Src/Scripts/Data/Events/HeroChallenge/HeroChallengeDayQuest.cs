using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroChallengeDayQuest : ScriptableObject
    {
        public HeroChallengeDayQuestData[] heroChallengeDayQuestDatas;

        [Serializable]
        public class HeroChallengeDayQuestData
        {
            public int day;
            public DayQuestInfo[] dayQuestInfos;
        }

        [Serializable]
        public class DayQuestInfo
        {
            public int questId;
            public int level;
            public Reward[] rewards;
        }
    }

#if UNITY_EDITOR

    public class HeroChallengeDayQuestPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/HeroAcademy/HeroChallenge/hero_challenge_day_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroChallengeDayQuest) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    HeroChallengeDayQuest gm = AssetDatabase.LoadAssetAtPath<HeroChallengeDayQuest>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroChallengeDayQuest>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.heroChallengeDayQuestDatas =
                        CsvReader.Deserialize<HeroChallengeDayQuest.HeroChallengeDayQuestData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}