using System.Collections.Generic;

namespace EW2
{
    public class UserAchievementQuest
    {
        public List<QuestUserData> listQuestData = new List<QuestUserData>();

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
            return listQuestData.Count <= 0;
        }


        public void CalculateQuest()
        {
            if (CanResetQuest())
            {
                var achievementQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<AchievementQuest>();

                if (achievementQuest != null)
                {
                    InsertData(achievementQuest.GetQuests());
                    GameContainer.Instance.Get<QuestManager>().LoadAchievementQuest();
                }

                UserData.Instance.Save();
            }
        }
    }
}