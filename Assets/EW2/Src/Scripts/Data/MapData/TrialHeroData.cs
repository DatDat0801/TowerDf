using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TrialHeroData : ScriptableObject
    {
        public TrialData[] trialDatas;

        [Serializable]
        public class TrialData
        {
            public int id;
            public int heroTrial;
            public int stageUnlock;
            public int modeStage;
            public int levelHero;
            public int levelSkillActive;
            public int levelSkillPassive1;
            public int levelSkillPassive2;
            public int levelSkillPassive3;
        }

        public TrialData GetDataTrial(int campaignId)
        {
            var idConvert = MapCampaignInfo.GetWorldMapModeId(campaignId);

            foreach (var trialData in trialDatas)
            {
                if (trialData.stageUnlock == idConvert.Item2)
                    return trialData;
            }

            return null;
        }
    }

#if UNITY_EDITOR
    public class TrialHeroDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Maps/trial_hero_config.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TrialHeroData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/MapCampaigns/" + nameAsset;
                    TrialHeroData gm = AssetDatabase.LoadAssetAtPath<TrialHeroData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TrialHeroData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    gm.trialDatas = CsvReader.Deserialize<TrialHeroData.TrialData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    //Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}