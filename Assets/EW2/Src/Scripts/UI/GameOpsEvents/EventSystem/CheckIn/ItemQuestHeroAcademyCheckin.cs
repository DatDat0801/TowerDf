using System;
using EnhancedUI.EnhancedScroller;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ItemQuestHeroAcademyCheckin : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtProgress;
        [SerializeField] private Text txtClaimed;
        [SerializeField] private TimeRemainUi txtTimeCountDown;
        [SerializeField] private Text txtDisable;
        [SerializeField] private Button btnClaim;
        [SerializeField] private Button btnDisable;
        [SerializeField] private Image imgProgress;
        [SerializeField] private Transform panelReward;
        [SerializeField] private Sprite[] arrImgProgress;

        private QuestItem questData;
        private Action claimCallback;
        private bool isNextQuest;

        private void Awake()
        {
            btnClaim.onClick.AddListener(ClaimCLick);
        }

        private void ClaimCLick()
        {
            if (!questData.IsCanReceive()) return;

            Reward.AddToUserData(questData.rewards, AnalyticsConstants.SourceHeroAcademyCheckin);
            PopupUtils.ShowReward(questData.rewards);
            questData.SetClaimed();
            claimCallback?.Invoke();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
        }

        public void InitData(QuestItem data, Action callback, bool isNextQuest)
        {
            this.questData = data;
            this.claimCallback = callback;
            this.isNextQuest = isNextQuest;
            ShowUi();
        }

        private void ShowUi()
        {
            txtTitle.text = string.Format(L.quest.quest_daily_0, questData.questLocalData.infoQuests[0].numberRequire);
            btnClaim.GetComponentInChildren<Text>().text = L.button.btn_claim;
            txtClaimed.text = L.button.reward_video_received;
            txtDisable.text = L.button.btn_claim;

            var ratio = questData.questUserData.count * 1f /
                questData.questLocalData.infoQuests[questData.currLevel].numberRequire * 1f;
            imgProgress.fillAmount = ratio;

            if (ratio < 1f)
            {
                txtProgress.text =
                    $"<color='{GameConfig.TextColorRed}'>{questData.questUserData.count}</color>/{questData.questLocalData.infoQuests[questData.currLevel].numberRequire}";
                imgProgress.sprite = arrImgProgress[0];
            }
            else
            {
                txtProgress.text =
                    $"{questData.questUserData.count}/{questData.questLocalData.infoQuests[questData.currLevel].numberRequire}";
                imgProgress.sprite = arrImgProgress[1];
            }

            btnClaim.gameObject.SetActive(questData.IsCanReceive());
            btnDisable.gameObject.SetActive(!questData.IsComplete());
            txtClaimed.gameObject.SetActive(questData.IsReceived());
            if (isNextQuest)
            {
                txtDisable.gameObject.SetActive(false);
                txtTimeCountDown.gameObject.SetActive(true);
                txtTimeCountDown.SetTimeRemain(TimeManager.GetEndTimeOfDaySeconds() - TimeManager.NowInSeconds);
            }
            else
            {
                txtDisable.gameObject.SetActive(true);
                txtTimeCountDown.gameObject.SetActive(false);
            }

            if (questData.rewards.Length > 0)
            {
                foreach (Transform child in panelReward)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < questData.rewards.Length; i++)
                {
                    var rewardUi = ResourceUtils.GetRewardUi(questData.rewards[i].type);
                    rewardUi.SetData(questData.rewards[i]);
                    rewardUi.SetParent(panelReward);
                }
            }
        }
    }
}