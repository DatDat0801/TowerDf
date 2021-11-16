using System;

namespace EW2
{
    [Serializable]
    public class QuestUserData
    {
        public int id;
        public int count;
        public int received;
        public int complete;
        public int level;

        public QuestUserData(int id, int count, int received, int complete)
        {
            this.id = id;
            this.count = count;
            this.received = received;
            this.complete = complete;
        }
    }
}