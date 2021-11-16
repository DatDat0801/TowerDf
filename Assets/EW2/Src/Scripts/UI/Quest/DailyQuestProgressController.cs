using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class DailyQuestProgressController : MonoBehaviour
    {
        private const float DistanceX = 1209f;
        private const float XStart = 553f;

        [SerializeField] private Image imgProgress;
        [SerializeField] private List<DailyQuestItemCheatUi> cheats;
        [SerializeField] private Text textCurrPoint;
        [SerializeField] private InfoCheatDailyQuestController popupInfo;

        private List<AchievementDailyQuest.AchievementDaily> listDatas;
        private int totalPoint;

        public void ShowUi()
        {
            GetData();
            
            for (int i = 0; i < cheats.Count; i++)
            {
                if (i >= listDatas.Count) continue;
                cheats[i].InitCheat(listDatas[i], CaculationPosCheat(listDatas[i].pointRequire));
                cheats[i].infoRewardCheat = ShowInfoReward;
            }

            var currPoint = UserData.Instance.UserDailyQuest.totalActivityPoint;
            textCurrPoint.text = $"{currPoint}";
            var ratio = currPoint * 1f / totalPoint * 1f;
            imgProgress.fillAmount = ratio;
        }

        private void GetData()
        {
            listDatas = GameContainer.Instance.Get<QuestManager>().Get<AchievementDailyQuest>().GetAchievements();
            totalPoint = listDatas[listDatas.Count - 1].pointRequire;
        }
        private void ShowInfoReward(AchievementDailyQuest.AchievementDaily data, Vector3 point)
        {
            popupInfo.SetReward(data.rewards, point);
            popupInfo.gameObject.SetActive(true);
        }

        private Vector3 CaculationPosCheat(int pointReq)
        {
            var ratio = pointReq * 1f / totalPoint * 1f;
            var posX = XStart + ratio * DistanceX;

            var posCheat = new Vector3(posX, 840f, 0);

            return posCheat;
        }

        public List<Reward> ClaimAll()
        {
            List<Reward> listRewards = new List<Reward>();
            for (int i = 0; i < cheats.Count; i++)
            {
                if (cheats[i].IsCanReceive())
                {
                    listRewards.AddRange(listDatas[i].rewards);
                    UserData.Instance.UserDailyQuest.listCheatOpened.Add(listDatas[i].achievementId);
                }
            }

            ShowUi();

            return listRewards;
        }
    }
}