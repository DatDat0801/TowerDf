using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [System.Serializable]
    public class StarRewardCampaignInfo
    {
        public int star;
        public Reward[] rewards;
    }
    
    [System.Serializable]
    public class RewardCampaignInfo
    {
        public StarRewardCampaignInfo[] starRewards;

        public RewardCampaignInfo(StarRewardCampaignInfo[] starRewards)
        {
            this.starRewards = starRewards;
        }

        public Reward[] GetWinReward()
        {
            if (this.starRewards.Length > 0)
            {
                return this.starRewards[0].rewards;
            }

            throw new Exception("Do not have win reward");
        }
        
        public Reward[] GetStarReward(int fromStar, int toStar)
        {
            if ((fromStar < toStar) && (this.starRewards.Length >= toStar))
            {
                var length = toStar - fromStar;
                var rewards = new Reward[length + 1][];

                for (int i = 0; i < length; i++)
                {
                    rewards[i] = GetStarReward(fromStar + i + 1);
                }

                rewards[length] = GetWinReward();

                return Reward.MergeRewards(rewards);
            }
            
            throw new Exception("Do not have star reward");
        }

        public Reward[] GetStarReward(int star)
        {
            if (this.starRewards.Length >= star)
            {
                    return this.starRewards[star].rewards;
            }
            
            throw new Exception("Do not have star reward");
        }
    }
    
    [System.Serializable]
    public class AllRewardCampaignInfo : ScriptableObject
    {
        public IntRewardCampaignDictionary rewardCampaigns;

        public RewardCampaignInfo GetRewardByCampaignId(int id)
        {
            if (rewardCampaigns.ContainsKey(id))
            {
                return rewardCampaigns[id];
            }
            
            throw new Exception($"CampaignId is not exist {id}");
        }
    }
    
#if UNITY_EDITOR
    

    [System.Serializable]
    public class RewardWorldMapCampaignInfo
    {
        public int worldId;
        public int mapId;
        public StarRewardCampaignInfo[] starRewards;
    }

    public class RewardCampaignInfoPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Maps/Campaign/WorldMapRewards/";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    string assetFile = "Assets/EW2/Resources/CSV/MapCampaigns/Rewards.asset";

                    var gm = AssetDatabase.LoadAssetAtPath<AllRewardCampaignInfo>(assetFile);

                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<AllRewardCampaignInfo>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }
                    else
                    {
                        gm.rewardCampaigns.Clear();
                    }

                    int numberMode = 2;
                    
                    for (int i = 0; i < numberMode; i++)
                    {
                        string csvFile = csvFormat + $"reward_{i}.csv";
                        TextAsset dataBase = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFile);

                        var allData = CsvReader.Deserialize<RewardWorldMapCampaignInfo>(dataBase.text);

                        foreach (var info in allData)
                        {
                            var campaignId = MapCampaignInfo.GetCampaignId(info.worldId, info.mapId, i);
                            gm.rewardCampaigns.Add(campaignId, new RewardCampaignInfo(info.starRewards));
                        }
                    }
                   
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);

                    // var rewardCampaign = gm.GetRewardByCampaignId(0);
                    //
                    // var rewards = rewardCampaign.GetStarReward(1, 3);
                }
            }
        }
    }
#endif
}