using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class DailyQuestItemCheatUi : MonoBehaviour
    {
        [SerializeField] private GameObject cheatClose;
        [SerializeField] private GameObject cheatOpen;
        [SerializeField] private GameObject[] vfx;
        [SerializeField] private Text txtPoint;
        [SerializeField] private Button btnCheat;

        private int cheatId;
        private AchievementDailyQuest.AchievementDaily dataCheat;
        private Vector3 posCheat;

        public Action<AchievementDailyQuest.AchievementDaily, Vector3> infoRewardCheat;

        private void Awake()
        {
            btnCheat.onClick.AddListener(CheatClick);
        }

        private void CheatClick()
        {
            if (IsCanReceive())
            {
                Reward.AddToUserData(dataCheat.rewards);
                PopupUtils.ShowReward(dataCheat.rewards);
                UserData.Instance.UserDailyQuest.listCheatOpened.Add(cheatId);
                UserData.Instance.Save();
                ShowUi();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
            }
            else
            {
                infoRewardCheat?.Invoke(dataCheat, posCheat);
            }
        }

        public void InitCheat(AchievementDailyQuest.AchievementDaily data, Vector3 point)
        {
            this.cheatId = data.achievementId;
            this.dataCheat = data;
            this.posCheat = point;
            GetComponent<RectTransform>().anchoredPosition3D = point;
            ShowUi();
        }

        private void ShowUi()
        {
            txtPoint.text = $"{dataCheat.pointRequire}";
            if (IsReceived())
            {
                cheatOpen.SetActive(true);
                cheatClose.SetActive(false);
                foreach (var effect in vfx)
                {
                    effect.SetActive(false);
                }
            }
            else
            {
                cheatOpen.SetActive(false);
                cheatClose.SetActive(true);
                if (IsCanReceive())
                {
                    foreach (var effect in vfx)
                    {
                        effect.SetActive(true);
                    }
                }
                else
                {
                    foreach (var effect in vfx)
                    {
                        effect.SetActive(false);
                    }
                }
            }
        }

        private bool IsComplete()
        {
            return UserData.Instance.UserDailyQuest.totalActivityPoint >= dataCheat.pointRequire;
        }

        private bool IsReceived()
        {
            return UserData.Instance.UserDailyQuest.listCheatOpened.Contains(cheatId);
        }

        public bool IsCanReceive()
        {
            return IsComplete() && !IsReceived();
        }
    }
}