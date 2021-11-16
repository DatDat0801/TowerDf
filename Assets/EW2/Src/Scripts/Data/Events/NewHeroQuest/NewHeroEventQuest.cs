using System.Collections.Generic;
using EW2.Events;

namespace EW2
{
    public class NewHeroEventQuest : IEventQuest
    {
        private Dictionary<int, QuestItem> mapQuest = new Dictionary<int, QuestItem>();

        public void ReadQuests()
        {
            mapQuest.Clear();
            var checkinData = GameContainer.Instance.Get<EventDatabase>().Get<NewHeroEventQuestDataBase>();

            foreach (var questInfo in checkinData.newHeroEventQuestBases)
            {
                var quest = GameContainer.Instance.Get<QuestManager>().CreateQuest(questInfo.questType);
                if (quest != null)
                {
                    quest.InitData(questInfo);
                    quest.Init();
                    mapQuest[quest.questId] = quest;
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
            foreach (var quest in mapQuest)
            {
                if (quest.Value.IsCanReceive())
                    count++;
            }

            return count;
        }

        public bool CheckCanReceive()
        {
            return GetCountCanReceive() > 0;
        }

        public List<QuestItem> GetAllQuests()
        {
            List<QuestItem> list = new List<QuestItem>();
            foreach (var item in mapQuest)
            {
                list.Add(item.Value);
            }

            return list;
        }
    }
}