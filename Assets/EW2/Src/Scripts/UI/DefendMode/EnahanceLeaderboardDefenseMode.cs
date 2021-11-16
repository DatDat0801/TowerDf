using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using ZitgaRankingDefendMode;

namespace EW2
{
    public class EnahanceLeaderboardDefenseMode : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private ItemRankingHeroDefense itemRewardPref;
        [SerializeField] private float sizeItem;
        [SerializeField] private EnhancedScroller vEnhancedScroller;

        private List<RankingDefense> _leaderboardDatas = new List<RankingDefense>();

        private bool _isInitScroll;

        public void SetData(List<RankingDefense> datas)
        {
            this._leaderboardDatas.Clear();
            this._leaderboardDatas.AddRange(datas);
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
            return this._leaderboardDatas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return this.sizeItem;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var itemController = scroller.GetCellView(itemRewardPref) as ItemRankingHeroDefense;

            if (itemController != null)
            {
                var data = this._leaderboardDatas[dataIndex];
                itemController.SetData(data);
                itemController.gameObject.SetActive(true);
                return itemController;
            }

            return null;
        }
    }
}