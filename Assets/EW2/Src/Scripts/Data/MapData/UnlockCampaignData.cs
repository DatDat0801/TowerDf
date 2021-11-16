using System.Collections.Generic;
using System.Linq;

namespace EW2
{
    public class UnlockCampaignData
    {
        private readonly Dictionary<int, int> dataUnlockDict;
        private readonly Dictionary<int, int> dataNextDict;

        public UnlockCampaignData(AllRewardCampaignInfo allReward)
        {
            dataUnlockDict = new Dictionary<int, int>();

            dataNextDict = new Dictionary<int, int>();

            var campaignIdList = allReward.rewardCampaigns.Keys.ToList();
            campaignIdList.Sort();

            dataUnlockDict[0] = -1;

            for (int i = 1; i < campaignIdList.Count; i++)
            {
                var step = 2 - (i % 2);
                dataUnlockDict.Add(campaignIdList[i], campaignIdList[i - step]);
            }

            for (int i = 0; i < campaignIdList.Count; i += 2)
            {
                if (i + 2 < campaignIdList.Count)
                {
                    dataNextDict.Add(campaignIdList[i], campaignIdList[i + 2]);
                }
                else
                {
                    dataNextDict.Add(campaignIdList[i], -1);
                }
            }
        }

        public int GetNextMap(int campaignId)
        {
            if (dataNextDict[campaignId] != -1)
            {
                return dataNextDict[campaignId];
            }
            //case press next button stage 19
            return campaignId + 100;
        }

        public int GetUnlockMap(int campaignId)
        {
            return dataUnlockDict[campaignId];
        }
    }
}