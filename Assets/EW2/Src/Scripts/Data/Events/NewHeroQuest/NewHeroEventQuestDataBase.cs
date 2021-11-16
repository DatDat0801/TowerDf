using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class NewHeroEventQuestBase : QuestBase
    {
        public void CopyQuestData(QuestBase dataCopy, int level)
        {
            this.questId = dataCopy.questId;
            this.questType = dataCopy.questType;
            this.targetType = dataCopy.targetType;
            if (this.infoQuests == null)
                this.infoQuests = new InfoQuest[] {
                    level < dataCopy.infoQuests.Length ? dataCopy.infoQuests[level] : dataCopy.infoQuests[0]
                };
            this.haveGoto = dataCopy.haveGoto;
        }
    }

    public class NewHeroEventQuestDataBase : ScriptableObject
    {
        public NewHeroEventQuestBase[] newHeroEventQuestBases;

        public NewHeroEventQuestBase GetQuestById(int questId)
        {
            foreach (var newHeroEventQuest in newHeroEventQuestBases)
            {
                if (newHeroEventQuest.questId == questId)
                {
                    return newHeroEventQuest;
                }
            }

            return null;
        }
    }

#if UNITY_EDITOR

    public class NewHeroEventQuestPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/NewHero/new_hero_quest.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(NewHeroEventQuestDataBase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    NewHeroEventQuestDataBase gm = AssetDatabase.LoadAssetAtPath<NewHeroEventQuestDataBase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<NewHeroEventQuestDataBase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.newHeroEventQuestBases = CsvReader.Deserialize<NewHeroEventQuestBase>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}