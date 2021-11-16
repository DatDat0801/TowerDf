using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class UserDailyQuest
    {
        public long timeResetQuest;
        public List<QuestUserData> listQuestData = new List<QuestUserData>();
        public int totalActivityPoint;
        public List<int> listCheatOpened = new List<int>();

        public void InsertData(List<QuestItem> listDatabase)
        {
            foreach (var questBase in listDatabase)
            {
                var questUserData = new QuestUserData(questBase.questId, 0, 0, 0);
                listQuestData.Add(questUserData);
            }
        }

        public bool CanResetQuest()
        {
            return GetTimeRemainReset() <= 0;
        }

        public void ResetQuest()
        {
            totalActivityPoint = 0;
            timeResetQuest = TimeManager.GetEndTimeOfDaySeconds();
            listQuestData.Clear();
            listCheatOpened.Clear();
        }

        public long GetTimeRemainReset()
        {
            return timeResetQuest - TimeManager.NowInSeconds;
        }

        public void CalculateQuest()
        {
            if (CanResetQuest())
            {
                ResetQuest();

                var dailyQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<DailyQuest>();

                if (dailyQuest != null)
                {
                    InsertData(dailyQuest.GetQuests());
                    GameContainer.Instance.Get<QuestManager>().LoadDailyQuest();
                }

                UserData.Instance.Save();
            }
        }
    }
}