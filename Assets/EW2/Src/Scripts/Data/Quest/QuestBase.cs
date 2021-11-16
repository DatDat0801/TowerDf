using System;
using System.Collections.Generic;

namespace EW2
{
    [Serializable]
    public class QuestBase
    {
        public int questId;
        public TargetType targetType;
        public QuestType questType;
        public int haveGoto;
        public InfoQuest[] infoQuests;
    }
    
    [Serializable]
    public class InfoQuest
    {
        public int level;
        public int limitReceive;
        public Reward[] rewards;
        public int numberRequire;
        public int stageUnlock;
        public int[] victim;
    }
}