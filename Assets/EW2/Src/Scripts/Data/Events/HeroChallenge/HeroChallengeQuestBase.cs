using System;

namespace EW2
{
    [Serializable]
    public class HeroChallengeQuestBase : QuestBase
    {
        public void CopyQuestData(QuestBase dataCopy, int level)
        {
            this.questId = dataCopy.questId;
            this.questType = dataCopy.questType;
            this.targetType = dataCopy.targetType;
            if (this.infoQuests == null)
                this.infoQuests = new InfoQuest[]{level < dataCopy.infoQuests.Length ? dataCopy.infoQuests[level] : dataCopy.infoQuests[0]};
            this.haveGoto = dataCopy.haveGoto;
        }
    }
}