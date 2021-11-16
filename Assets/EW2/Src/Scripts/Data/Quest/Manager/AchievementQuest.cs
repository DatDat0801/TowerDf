using System.Collections.Generic;
using TigerForge;

namespace EW2
{
    public class AchievementQuest : IEventQuest
    {
        private Dictionary<int, QuestItem> mapQuest = new Dictionary<int, QuestItem>();

        public void ReadQuests()
        {
            mapQuest.Clear();
            var dailyDataBase = GameContainer.Instance.Get<QuestManager>().Get<AchievementDataBase>();
            var listQuest = dailyDataBase.GetListDailyQuest();

            foreach (var questBase in listQuest)
            {
                var quest = GameContainer.Instance.Get<QuestManager>().CreateQuest(questBase.questType);
                if (quest != null)
                {
                    quest.InitData(questBase);
                    quest.Init();
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
            return GetCountCanReceive() > 0;
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

        public QuestItem GetQuest(int questId)
        {
            foreach (var item in mapQuest)
            {
                if (item.Key == questId)
                    return item.Value;
            }

            return null;
        }
    }
}