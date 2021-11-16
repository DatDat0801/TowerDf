using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.Quest
{
    public class DailyQuestContainer : TabContainer, IEnhancedScrollerDelegate
    {
        [SerializeField] private Button btnClaimAll;
        [SerializeField] private TimeCountDownUi timeCountdown;
        [SerializeField] private float itemSize;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private DailyQuestProgressController progressUi;

        private List<QuestItem> listQuest = new List<QuestItem>();

        private void Awake()
        {
            btnClaimAll.onClick.AddListener(ClaimAllClick);
        }

        private void ClaimAllClick()
        {
            List<Reward> listRewards = new List<Reward>();
            listRewards.AddRange(progressUi.ClaimAll());
            listRewards.AddRange(ClaimAllQuest());

            if (listRewards.Count > 0)
            {
                var rewardMerger = Reward.MergeRewards(listRewards.ToArray());
                Reward.AddToUserData(rewardMerger);
                PopupUtils.ShowReward(rewardMerger);
                HandleClaimQuest();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
            }
        }

        private List<Reward> ClaimAllQuest()
        {
            List<Reward> listRewards = new List<Reward>();

            foreach (var questItem in listQuest)
            {
                if (questItem.IsCanReceive())
                {
                    listRewards.AddRange(questItem.rewards);
                    questItem.SetClaimed();
                }
            }

            return listRewards;
        }

        private void GetData()
        {
            listQuest.Clear();
            var dailyQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<DailyQuest>();
            listQuest.AddRange(SortData(dailyQuest.GetQuests()));
        }

        private List<QuestItem> SortData(List<QuestItem> lstInput)
        {
            var lstResult = new List<QuestItem>();
            lstResult = lstInput.OrderByDescending(quest => quest.IsComplete() == false).ToList();
            lstResult = lstResult.OrderByDescending(quest => quest.IsCanReceive() == true).ToList();
            return lstResult;
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            GetData();
            InitScroll();
            ShowUi();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void ShowUi()
        {
            btnClaimAll.GetComponentInChildren<Text>().text = L.button.claim_all;
            timeCountdown.SetTitle(L.popup.reset_in_txt);
            timeCountdown.SetData(UserData.Instance.UserDailyQuest.GetTimeRemainReset(), default,
                () =>
                {
                    UserData.Instance.UserDailyQuest.CalculateQuest();
                    ShowContainer();
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                });
            if (progressUi) progressUi.ShowUi();
        }

        private void HandleClaimQuest()
        {
            listQuest = SortData(listQuest);
            scroller.ReloadData();
            if (progressUi) progressUi.ShowUi();
        }

        #region Scroller event

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
            var questUi = scroller.GetCellView(cellView) as ItemDailyQuestUi;

            questUi.dataIndex = dataIndex;

            var data = listQuest[dataIndex];

            questUi.InitData(data, HandleClaimQuest);

            return questUi;
        }

        #endregion
    }
}