using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroChallengeQuest : ScriptableObject
    {
        public HeroChallengeQuestBase[] heroChallengeQuestBases;

        public HeroChallengeQuestBase GetQuestById(int questId)
        {
            foreach (var heroChallengeQuest in heroChallengeQuestBases)
            {
                if (heroChallengeQuest.questId == questId)
                {
                    return heroChallengeQuest;
                }
            }

            return null;
        }
    }

#if UNITY_EDITOR

    public class HeroChallengeQuestPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/HeroAcademy/HeroChallenge/hero_challenge_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroChallengeQuest) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    HeroChallengeQuest gm = AssetDatabase.LoadAssetAtPath<HeroChallengeQuest>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroChallengeQuest>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.heroChallengeQuestBases = CsvReader.Deserialize<HeroChallengeQuestBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}