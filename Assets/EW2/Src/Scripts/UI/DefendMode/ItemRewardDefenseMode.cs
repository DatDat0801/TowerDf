using EnhancedUI.EnhancedScroller;
using EW2.Tools;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemRewardDefenseMode : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Transform panelReward;

        private DefendModeReward.DefendModeRewardData _dataItem;
        private bool _IsEndReward;

        public void InitData(DefendModeReward.DefendModeRewardData data, bool endReward)
        {
            _dataItem = data;
            this._IsEndReward = endReward;
            ShowUi();
        }

        private void ShowUi()
        {
            if (!this._IsEndReward)
            {
                txtTitle.text = this._dataItem.wave.ToString();
            }
            else
            {
                txtTitle.text = $"{this._dataItem.wave}+";
            }

            if (this._dataItem.rewards.Length > 0)
            {
                this.panelReward.DespawnAllChildren();

                foreach (var reward in this._dataItem.rewards)
                {
                    var rewardUi = ResourceUtils.GetRewardUi(reward.type);
                    rewardUi.SetData(reward);
                    rewardUi.SetParent(panelReward);
                }
            }
        }
    }
}