using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class QuestContainer : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private float itemSize;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller scroller;
        private List<QuestItem> listQuest = new List<QuestItem>();
        private Action refreshCallback;

        public void RefreshTab(Action callback)
        {
            this.refreshCallback = callback;
        }

        public void ShowQuest(List<QuestItem> datas)
        {
            listQuest.Clear();
            listQuest.AddRange(SortData(datas));
            InitScroll();
        }

        private List<QuestItem> SortData(List<QuestItem> lstInput)
        {
            var lstResult = new List<QuestItem>();
            lstResult = lstInput.OrderByDescending(quest => quest.IsComplete() == false).ToList();
            lstResult = lstResult.OrderByDescending(quest => quest.IsCanReceive() == true).ToList();
            return lstResult;
        }

        private void HandleClaimQuest()
        {
            refreshCallback?.Invoke();
        }

        private void InitScroll()
        {
            if (scroller.Delegate == null)
                scroller.Delegate = this;
            else
                scroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (listQuest == null)
                return 0;

            return listQuest.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return itemSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var questUi = scroller.GetCellView(cellView) as ItemQuestHeroChallenge;

            questUi.dataIndex = dataIndex;

            var data = listQuest[dataIndex];

            questUi.InitData(data, HandleClaimQuest);

            return questUi;
        }
    }
}