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
    public class ItemQuestHeroChallenge : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtProgress;
        [SerializeField] private Text txtClaimed;
        [SerializeField] private Button btnClaim;
        [SerializeField] private Button btnGoto;
        [SerializeField] private Image imgProgress;
        [SerializeField] private Transform panelReward;
        [SerializeField] private Sprite[] arrImgProgress;

        private QuestItem questData;
        private Action claimCallback;

        private void Awake()
        {
            btnClaim.onClick.AddListener(ClaimCLick);
            btnGoto.onClick.AddListener(GoClick);
        }

        private void GoClick()
        {
            if (UserData.Instance.CampaignData.HighestResultLevel() <
                questData.questLocalData.infoQuests[questData.currLevel].stageUnlock)
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    questData.questLocalData.infoQuests[questData.currLevel].stageUnlock + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            questData.GoToTarget();
        }

        private void ClaimCLick()
        {
            if (!questData.IsCanReceive()) return;

            Reward.AddToUserData(questData.rewards, AnalyticsConstants.SourceHeroChallenge, UserData.Instance.UserEventData.HeroChallengeUserData.currentDay.ToString());
            PopupUtils.ShowReward(questData.rewards);
            questData.SetClaimed();
            claimCallback?.Invoke();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
            EventManager.EmitEvent(GamePlayEvent.OnRepaintGloryRoad);
        }

        public void InitData(QuestItem data, Action callback)
        {
            questData = data;
            this.claimCallback = callback;
            ShowUi();
        }

        private void ShowUi()
        {
            txtTitle.text = GetNameQuest();
            btnClaim.GetComponentInChildren<Text>().text = L.button.btn_claim;
            btnGoto.GetComponentInChildren<Text>().text = L.button.go_to_btn;
            txtClaimed.text = L.button.reward_video_received;

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

            if (questData.questLocalData.haveGoto <= 0)
                btnGoto.gameObject.SetActive(false);
            else
                btnGoto.gameObject.SetActive(!questData.IsComplete());

            txtClaimed.gameObject.SetActive(questData.IsReceived());

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

        private string GetNameQuest()
        {
            var idCompare = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroChallengeQuestEvent>()
                .GetQuestId(questData.questId);

            if (idCompare.Item2 == 13)
                idCompare.Item2 = 2;

            var nameFormat = Localization.Current.Get("quest", $"quest_hero_challenge_{idCompare.Item2}");

            switch (idCompare.Item2)
            {
                case 0:
                case 1:
                case 5:
                case 6:
                case 7:
                case 15:
                    return string.Format(nameFormat, questData.victim[0]);
                case 2:
                case 13:
                    var nameDifficult = L.stages.difficult_normal;
                    if (questData.targetType == TargetType.AnyMapNightmare)
                        nameDifficult = L.stages.difficult_nightmare;
                    return string.Format(nameFormat, questData.questLocalData.infoQuests[0].numberRequire,
                        nameDifficult);
                default:
                    return string.Format(nameFormat, questData.questLocalData.infoQuests[0].numberRequire);
            }
        }
    }
}