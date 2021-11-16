using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class DefendModeReward : ScriptableObject
    {
        public DefendModeRewardData[] defendModeRewardDatas;

        [Serializable]
        public class DefendModeRewardData
        {
            public int wave;
            public Reward[] rewards;
        }

        public Reward[] GetRewards(int wave)
        {
            var result = new Reward[0];
            for (int i = 0; i < defendModeRewardDatas.Length; i++)
            {
                if (this.defendModeRewardDatas[i].wave <= wave)
                    result = Reward.MergeRewards(this.defendModeRewardDatas[i].rewards, result);
            }

            return result;
        }
        
    }

#if UNITY_EDITOR

    public class DefendModeRewardProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/HeroDefendMode/hero_defend_mode_rewards.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(DefendModeReward) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/HeroDefendMode/" + nameAsset;

                    // data orgin
                    DefendModeReward gm = AssetDatabase.LoadAssetAtPath<DefendModeReward>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DefendModeReward>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.defendModeRewardDatas = CsvReader.Deserialize<DefendModeReward.DefendModeRewardData>(data.text);

                    EditorUtility.SetDirty(gm);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}