using System.Collections.Generic;
using EW2.Events;

namespace EW2
{
    public class HeroChallengeQuestEvent : IEventQuest
    {
        private Dictionary<int, QuestItem> mapQuest = new Dictionary<int, QuestItem>();

        public void ReadQuests()
        {
            mapQuest.Clear();
            var heroChallengeQuest = GameContainer.Instance.Get<EventDatabase>().Get<HeroChallengeQuest>();
            var heroChallengeDayQuest = GameContainer.Instance.Get<EventDatabase>().Get<HeroChallengeDayQuest>();

            foreach (var dayQuest in heroChallengeDayQuest.heroChallengeDayQuestDatas)
            {
                foreach (var questInfo in dayQuest.dayQuestInfos)
                {
                    var questData = heroChallengeQuest.GetQuestById(questInfo.questId);
                    if (questData != null)
                    {
                        var questFocus = new HeroChallengeQuestBase();
                        questFocus.CopyQuestData(questData, questInfo.level);
                        var quest = GameContainer.Instance.Get<QuestManager>().CreateQuest(questFocus.questType);
                        if (quest != null)
                        {
                            quest.InitData(questFocus);
                            quest.questId = GetQuestIdConvert(dayQuest.day, questInfo.questId, questInfo.level);
                            quest.rewards = questInfo.rewards;
                            quest.Init();
                            mapQuest[quest.questId] = quest;
                        }
                    }
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
                var questIdCompare = GetQuestId(quest.Key);
                if ((questIdCompare.Item1 + 1) <= UserData.Instance.UserEventData.HeroChallengeUserData.currentDay &&
                    quest.Value.IsCanReceive())
                    count++;
            }

            return count;
        }

        public int GetCountCanReceiveByDay(int day)
        {
            var count = 0;
            foreach (var quest in mapQuest)
            {
                var questIdCompare = GetQuestId(quest.Key);

                if (questIdCompare.Item1 == (day - 1) && quest.Value.IsCanReceive())
                    count++;
            }

            return count;
        }

        public List<QuestItem> GetQuestsByDay(int day)
        {
            List<QuestItem> list = new List<QuestItem>();
            foreach (var item in mapQuest)
            {
                var questIdCompare = GetQuestId(item.Key);
                if (questIdCompare.Item1 == (day - 1))
                    list.Add(item.Value);
            }

            return list;
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

        public bool CheckCanReceive()
        {
            return GetCountCanReceive() > 0;
        }

        public int GetQuestIdConvert(int day, int questId, int level)
        {
            return day * 10000 + questId * 100 + level;
        }

        public (int, int, int) GetQuestId(int questIdConvert)
        {
            int level = questIdConvert % 100;

            int questId = (questIdConvert % 10000) / 100;

            int day = questIdConvert / 10000;

            return (day, questId, level);
        }
    }
}