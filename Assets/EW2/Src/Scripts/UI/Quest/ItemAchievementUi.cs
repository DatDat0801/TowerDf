using System;
using EnhancedUI.EnhancedScroller;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ItemAchievementUi : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtProgress;
        [SerializeField] private Text txtClaimed;
        [SerializeField] private Button btnClaim;
        [SerializeField] private Button btnGoto;
        [SerializeField] private Image imgProgress;
        [SerializeField] private Transform panelReward;

        private QuestItem questData;
        private Action claimCallback;

        private void Awake()
        {
            btnClaim.onClick.AddListener(ClaimCLick);
            btnGoto.onClick.AddListener(GoClick);
        }

        private void GoClick()
        {
            questData.GoToTarget();
        }

        private void ClaimCLick()
        {
            if (!questData.IsCanReceive()) return;

            var reward = questData.rewards[0];
            reward.AddToUserData(true, AnalyticsConstants.SourceAchievement);
            PopupUtils.ShowReward(reward);
            questData.SetClaimed();
            claimCallback?.Invoke();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
        }

        public void InitData(QuestItem data, Action callback)
        {
            questData = data;
            this.claimCallback = callback;
            ShowUi();
        }

        private void ShowUi()
        {
            var name = Localization.Current.Get("achievement", $"achievement_{questData.questId}");
            txtTitle.text = string.Format(name, questData.questLocalData.infoQuests[questData.currLevel].numberRequire);
            btnClaim.GetComponentInChildren<Text>().text = L.button.btn_claim;
            btnGoto.GetComponentInChildren<Text>().text = L.button.go_to_btn;
            txtClaimed.text = L.button.reward_video_received;

            var ratio = questData.questUserData.count * 1f /
                questData.questLocalData.infoQuests[questData.currLevel].numberRequire * 1f;
            imgProgress.fillAmount = ratio;

            if (ratio < 1f)
            {
                txtProgress.text =
                    $"<color='#E91B24'>{questData.questUserData.count}</color><color='#FFFFFF'>/{questData.questLocalData.infoQuests[questData.currLevel].numberRequire}</color>";
            }
            else
            {
                txtProgress.text =
                    $"{questData.questUserData.count}/{questData.questLocalData.infoQuests[questData.currLevel].numberRequire}";
            }

            btnClaim.gameObject.SetActive(questData.IsCanReceive());

            if (questData.questLocalData.haveGoto <= 0)
                btnGoto.gameObject.SetActive(false);
            else
                btnGoto.gameObject.SetActive(!questData.IsComplete());

            txtClaimed.gameObject.SetActive(questData.IsReceived());

            if (questData.rewards.Length > 0)
            {
                foreach (Transform child in panelReward)
                {
                    LeanPool.Despawn(child.gameObject);
                }

                var rewardUi = ResourceUtils.GetRewardUi(questData.rewards[0].type);

                rewardUi.SetData(questData.rewards[0]);

                rewardUi.SetParent(panelReward);
            }
        }
    }
}