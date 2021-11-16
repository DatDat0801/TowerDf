using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using Invoke;
using UnityEngine;
using ZitgaTournamentMode;

namespace EW2
{
    public class EnahanceLeaderboardTournament : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private ItemRankingTournament itemRewardPref;
        [SerializeField] private EnhancedScroller vEnhancedScroller;

        private List<RankingTournament> _leaderboardDatas = new List<RankingTournament>();

        private bool _isInitScroll;

        private int _currItemShowMore;

        private LeaderboardArenaTabId _currTab;
        public void SetData(List<RankingTournament> datas, LeaderboardArenaTabId arenaTabId)
        {
            this._currTab = arenaTabId;
            this._currItemShowMore = -1;
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
            if (this._leaderboardDatas[dataIndex].IsShowMore)
            {
                return 275f;
            }
            else
            {
                return 145f;
            }
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var itemController = scroller.GetCellView(itemRewardPref) as ItemRankingTournament;

            if (itemController != null)
            {
                var data = this._leaderboardDatas[dataIndex];
                itemController.SetData(dataIndex, data, ItemClick, this._currTab);
                itemController.gameObject.SetActive(true);
                return itemController;
            }

            return null;
        }

        private void ItemClick(int dataIndex)
        {
            for (int i = 0; i < this._leaderboardDatas.Count; i++)
            {
                if (i == dataIndex)
                {
                    if (dataIndex != this._currItemShowMore)
                    {
                        this._currItemShowMore = dataIndex;
                        this._leaderboardDatas[i].IsShowMore = true;
                    }
                    else
                    {
                        this._currItemShowMore = -1;
                        this._leaderboardDatas[i].IsShowMore = false;
                    }
                }
                else
                {
                    this._leaderboardDatas[i].IsShowMore = false;
                }
            }

            this.vEnhancedScroller.ReloadData();
            this.vEnhancedScroller.JumpToDataIndex(dataIndex);
        }
    }
}