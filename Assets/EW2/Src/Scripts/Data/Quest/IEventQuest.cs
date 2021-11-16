using System.Collections.Generic;

namespace EW2
{
    public interface IEventQuest
    {
        void ReadQuests();
        void UpdateDataQuests(List<QuestUserData> data);
        int GetCountCanReceive();
        bool CheckCanReceive();
    }
}