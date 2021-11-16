using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TournamentReward : ScriptableObject
    {
        public TournamentRewardData[] tournamentRewardDatas;

        [Serializable]
        public class TournamentRewardData
        {
            public int rank;
            public Reward[] rewards;
        }

        public TournamentRewardData GetRewardData(int rankId)
        {
            foreach (var tournamentReward in this.tournamentRewardDatas)
            {
                if (tournamentReward.rank == rankId)
                {
                    return tournamentReward;
                }
            }

            return null;
        }
    }

#if UNITY_EDITOR

    public class TournamentRewardProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament/tournament_rewards.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TournamentReward) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tournament/" + nameAsset;

                    // data orgin
                    TournamentReward gm = AssetDatabase.LoadAssetAtPath<TournamentReward>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TournamentReward>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.tournamentRewardDatas = CsvReader.Deserialize<TournamentReward.TournamentRewardData>(data.text);

                    EditorUtility.SetDirty(gm);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}