using System.Collections.Generic;
using EW2.Events;

namespace EW2
{
    public class HeroAcademyUserData : BaseBundleData
    {
        public bool receivedExchange;
        public List<QuestUserData> listQuestData = new List<QuestUserData>();

        public void InsertData(List<QuestItem> listDatabase)
        {
            foreach (var questBase in listDatabase)
            {
                var questUserData = new QuestUserData(questBase.questId, 0, 0, 0);
                listQuestData.Add(questUserData);
            }
        }

        public override bool CheckCanShow()
        {
            var heroAcademyDataBase = GameContainer.Instance.Get<EventDatabase>().Get<HeroAcademyCondition>();

            if (!isOpen)
            {
                if (this.timeEnd <= 0)
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEndCaculate = TimeManager.NowInSeconds + heroAcademyDataBase.packConditions[0].duration;
                    SetTimeEnd(timeEndCaculate);
                    UserData.Instance.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (TimeRemain() > 0)
                {
                    return true;
                }
                else
                {
                    isOpen = false;
                    UserData.Instance.Save();
                    return false;
                }
            }
        }
    }
}