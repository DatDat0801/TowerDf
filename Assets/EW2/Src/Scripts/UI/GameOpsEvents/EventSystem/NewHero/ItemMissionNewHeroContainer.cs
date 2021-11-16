using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ItemMissionNewHeroContainer : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private float itemSize;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private Button btnClaimAll;
        [SerializeField] private Button btnClaimAllDisable;

        private List<QuestItem> _listQuest = new List<QuestItem>();
        private Action _refreshCallback;

        private void Awake()
        {
            this.btnClaimAll.onClick.AddListener(ClaimAllClick);
        }

        private void ClaimAllClick()
        {
            List<Reward> listRewards = new List<Reward>();
            listRewards.AddRange(ClaimAllQuest());

            if (listRewards.Count > 0)
            {
                var rewardMerger = Reward.MergeRewards(listRewards.ToArray());
                Reward.AddToUserData(rewardMerger, AnalyticsConstants.SourceNewHeroMissionEvent);
                PopupUtils.ShowReward(rewardMerger);
                HandleClaimQuest();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
            }
        }

        private List<Reward> ClaimAllQuest()
        {
            List<Reward> listRewards = new List<Reward>();

            foreach (var questItem in _listQuest)
            {
                if (questItem.IsCanReceive())
                {
                    listRewards.AddRange(questItem.rewards);
                    questItem.SetClaimed();
                }
            }

            return listRewards;
        }

        public void RefreshTab(Action callback)
        {
            this._refreshCallback = callback;
        }

        public void ShowQuest(List<QuestItem> datas)
        {
            this._listQuest.Clear();
            this._listQuest.AddRange(SortData(datas));
            
            InitScroll();
            
            this.btnClaimAll.GetComponentInChildren<Text>().text = L.button.claim_all;
            this.btnClaimAllDisable.GetComponentInChildren<Text>().text = L.button.claim_all;
           
            var haveQuestComplete = GameContainer.Instance.Get<QuestManager>().GetQuest<NewHeroEventQuest>()
                .CheckCanReceive();
            this.btnClaimAll.gameObject.SetActive(haveQuestComplete);
            this.btnClaimAllDisable.gameObject.SetActive(!haveQuestComplete);
        }

        private List<QuestItem> SortData(List<QuestItem> lstInput)
        {
            var lstResult = lstInput.OrderByDescending(quest => quest.IsComplete() == false).ToList();
            lstResult = lstResult.OrderByDescending(quest => quest.IsCanReceive() == true).ToList();
            return lstResult;
        }

        private void HandleClaimQuest()
        {
            this._refreshCallback?.Invoke();
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
            if (this._listQuest == null)
                return 0;

            return this._listQuest.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return itemSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var questUi = scroller.GetCellView(cellView) as ItemQuestNewHero;

            if (questUi != null)
            {
                questUi.dataIndex = dataIndex;

                var data = this._listQuest[dataIndex];

                questUi.InitData(data, HandleClaimQuest);

                return questUi;
            }

            return null;
        }
    }
}