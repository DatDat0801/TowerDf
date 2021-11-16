using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class DailyQuest : IEventQuest
    {
        private Dictionary<int, QuestItem> mapQuest = new Dictionary<int, QuestItem>();

        public void ReadQuests()
        {
            mapQuest.Clear();
            var dailyDataBase = GameContainer.Instance.Get<QuestManager>().Get<DailyQuestDataBase>();
            var listQuest = dailyDataBase.GetListDailyQuest();
            foreach (var questBase in listQuest)
            {
                var quest = GameContainer.Instance.Get<QuestManager>().CreateQuest(questBase.questType);
                if (quest != null)
                {
                    quest.InitData(questBase);
                    quest.Init();
                    quest.onQuestComplete = () =>
                    {
                        UserData.Instance.UserDailyQuest.totalActivityPoint += questBase.activityPoint;
                        EventManager.EmitEvent(GamePlayEvent.OnCompleteDailyQuest);
                        CheckCompleteAll();
                    };
                    mapQuest[questBase.questId] = quest;
                }
            }
        }

        public void UpdateDataQuests(List<QuestUserData> data)
        {
            foreach (var questUserData in data)
            {
                if (mapQuest.ContainsKey(questUserData.id))
                    mapQuest[questUserData.id].InsertData(questUserData);
            }
        }

        public int GetCountCanReceive()
        {
            var count = 0;
            foreach (var quest in mapQuest.Values)
            {
                if (quest.IsCanReceive())
                    count++;
            }

            return count;
        }

        public bool CheckCanReceive()
        {
            return GetCountCanReceive() > 0 || GetCheatCanReceive() > 0;
        }

        public List<QuestItem> GetQuests()
        {
            List<QuestItem> list = new List<QuestItem>();
            foreach (var item in mapQuest)
            {
                list.Add(item.Value);
            }

            return list;
        }

        private int GetCheatCanReceive()
        {
            var listDatas = GameContainer.Instance.Get<QuestManager>().Get<AchievementDailyQuest>().GetAchievements();

            var count = 0;
            foreach (var achievementDaily in listDatas)
            {
                if (UserData.Instance.UserDailyQuest.totalActivityPoint >= achievementDaily.pointRequire &&
                    !UserData.Instance.UserDailyQuest.listCheatOpened.Contains(achievementDaily.achievementId))
                    count++;
            }

            return count;
        }

        private void CheckCompleteAll()
        {
            var isComplete = true;

            foreach (var quest in mapQuest.Values)
            {
                if (!quest.IsComplete())
                {
                    isComplete = false;
                    break;
                }
            }

            if (isComplete)
                EventManager.EmitEvent(GamePlayEvent.OnCompleteAllDailyQuest);
        }
    }
}