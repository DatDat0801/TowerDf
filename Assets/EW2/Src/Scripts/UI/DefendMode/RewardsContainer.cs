using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class RewardsContainer : TabContainer
    {
        [SerializeField] private Text txtTitleWaves;
        [SerializeField] private Text txtTitleRewards;
        [SerializeField] private EnahanceRewardDefenseMode rewardDefense;

        List<DefendModeReward.DefendModeRewardData> _rewardDatas = new List<DefendModeReward.DefendModeRewardData>();
        private bool _isInited;

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            this.txtTitleWaves.text = L.gameplay.ingame_wave;
            this.txtTitleRewards.text = L.gameplay.rewards;
            if (!this._isInited)
            {
                this._isInited = true;
                GetRewardData();
            }

            rewardDefense.SetData(this._rewardDatas);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void GetRewardData()
        {
            var dataBase = GameContainer.Instance.Get<DefendModeDataBase>().Get<DefendModeReward>();
            if (dataBase != null)
            {
                var rewards = dataBase.defendModeRewardDatas;
                foreach (var reward in rewards)
                {
                    this._rewardDatas.Add(reward);
                }
            }
        }
    }
}