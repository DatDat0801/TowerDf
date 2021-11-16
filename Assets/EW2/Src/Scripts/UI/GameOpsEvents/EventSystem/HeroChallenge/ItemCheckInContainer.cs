using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class ItemCheckInContainer : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private float itemSize;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller scroller;
        private List<QuestItem> listQuest = new List<QuestItem>();
        private Action refreshCallback;
        private int indexNextQuest;

        public void RefreshTab(Action callback)
        {
            this.refreshCallback = callback;
        }

        public void ShowQuest(List<QuestItem> datas)
        {
            listQuest.Clear();
            listQuest.AddRange(datas);
            indexNextQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroAcademyCheckinEvent>()
                .GetIndexNextQuest();
            InitScroll();
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
            var questUi = scroller.GetCellView(cellView) as ItemQuestHeroAcademyCheckin;

            questUi.dataIndex = dataIndex;

            var data = listQuest[dataIndex];

            questUi.InitData(data, HandleClaimQuest, dataIndex == indexNextQuest);

            return questUi;
        }
    }
}