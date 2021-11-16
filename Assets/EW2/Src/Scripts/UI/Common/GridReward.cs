using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class GridReward
    {
        private readonly Transform root;
        
        private Reward[] data;
        
        private readonly List<RewardUI> uiList;

        public GridReward(Transform root)
        {
            this.root = root;
            
            uiList = new List<RewardUI>();
        }

        public void SetData(List<Reward> data)
        {
            SetData(data.ToArray());
        }

        public void SetData(Reward[] data)
        {
            this.data = data;
            
            foreach (var reward in data)
            {
                if(reward.number <= 0)
                    continue;
                
                var ui = ResourceUtils.GetRewardUi(reward.type);
                
                ui.SetData(reward);
                
                ui.SetParent(root);
                
                uiList.Add(ui);
            }
        }

        public void ReturnPool()
        {
            foreach (var rewardUi in uiList)
            {
                rewardUi.ReturnPool();
            }
            
            uiList.Clear();
        }
    }
}