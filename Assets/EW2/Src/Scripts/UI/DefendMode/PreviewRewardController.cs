using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class PreviewRewardController : MonoBehaviour
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Button btnView;

        private DefendModeReward.DefendModeRewardData _rewardData;
        private List<Reward> _rewards = new List<Reward>();

        private void Awake()
        {
            this.btnView.onClick.AddListener(ViewClick);
        }

        public void SetData(int currWaveCleared)
        {
            GetData(currWaveCleared);
            this.txtTitle.text = string.Format(L.playable_mode.reward_milestone_txt, this._rewardData.wave);
        }

        private void GetData(int currWave)
        {
            var rewardsData = GameContainer.Instance.Get<DefendModeDataBase>().Get<DefendModeReward>()
                .defendModeRewardDatas;

            this._rewards.Clear();

            foreach (var reward in rewardsData)
            {
                this._rewards.AddRange(reward.rewards);

                if (reward.wave > currWave)
                {
                    this._rewardData = reward;
                    break;
                }
            }

            if (this._rewardData == null)
                this._rewardData = rewardsData[rewardsData.Length - 1];
        }

        private void ViewClick()
        {
            var rewardShow = Reward.MergeRewards(this._rewards.ToArray());
            var data = new MultiRewardWindowProperties(rewardShow);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_preview_reward_defense_mode, data);
        }
    }
}