using System.Collections.Generic;
using System.Linq;
using EW2.Events;
using SocialTD2;

namespace EW2
{
    public class HeroChallengeUserData
    {
        public long timeResetNewDay;
        public int currentDay;
        public Dictionary<int, QuestUserData> dictQuestData = new Dictionary<int, QuestUserData>();
        private List<QuestUserData> _listDatabase = new List<QuestUserData>();

        public void InsertData(List<QuestItem> listDatabase)
        {
            foreach (var questBase in listDatabase)
            {
                if (!this.dictQuestData.Keys.Contains(questBase.questId))
                {
                    var questUserData = new QuestUserData(questBase.questId, 0, 0, 0);
                    this.dictQuestData.Add(questBase.questId, questUserData);
                }
            }
        }

        public List<QuestUserData> GetListQuestUserData()
        {
            _listDatabase.Clear();

            foreach (var questBase in this.dictQuestData.Values)
            {
                this._listDatabase.Add(questBase);
            }

            return this._listDatabase;
        }

        public bool CanUnlockNewDay()
        {
            var heroChallengeDayQuest = GameContainer.Instance.Get<EventDatabase>().Get<HeroChallengeDayQuest>();

            return GetTimeRemainReset() <= 0 && currentDay < heroChallengeDayQuest.heroChallengeDayQuestDatas.Length;
        }

        public void ResetQuest()
        {
            currentDay++;
            timeResetNewDay = TimeManager.GetEndTimeOfDaySeconds();
        }

        public long GetTimeRemainReset()
        {
            return timeResetNewDay - TimeManager.NowInSeconds;
        }

        public void CalculateQuest()
        {
            if (CanUnlockNewDay())
            {
                ResetQuest();
                UserData.Instance.Save();
                LoadSaveUtilities.AutoSave(false);
            }
        }
    }
}