using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class EnahanceRewardDefenseMode : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScrollerCellView itemRewardPref;
        [SerializeField] private float sizeItem;
        [SerializeField] private EnhancedScroller vEnhancedScroller;

        private List<DefendModeReward.DefendModeRewardData> _rewardDatas =
            new List<DefendModeReward.DefendModeRewardData>();

        private bool _isInitScroll;

        public void SetData(List<DefendModeReward.DefendModeRewardData> datas)
        {
            this._rewardDatas.Clear();
            this._rewardDatas.AddRange(datas);
            if (!this._isInitScroll)
            {
                this._isInitScroll = true;
                this.vEnhancedScroller.Delegate = this;
            }
            else
            {
                this.vEnhancedScroller.ReloadData();
            }
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return this._rewardDatas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return this.sizeItem;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var itemController = scroller.GetCellView(itemRewardPref) as ItemRewardDefenseMode;

            if (itemController != null)
            {
                var data = this._rewardDatas[dataIndex];
                itemController.InitData(data, (dataIndex == this._rewardDatas.Count - 1));
                itemController.gameObject.SetActive(true);
                return itemController;
            }

            return null;
        }
    }
}