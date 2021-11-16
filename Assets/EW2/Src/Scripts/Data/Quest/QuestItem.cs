using System;
using TigerForge;

namespace EW2
{
    public class QuestItem
    {
        public int questId;
        public QuestType questType;
        public TargetType targetType;
        public int limitReceive;
        public int count;
        public Reward[] rewards;
        public QuestUserData questUserData;
        public QuestBase questLocalData;
        public Action onQuestComplete;
        public int currLevel;
        public int[] victim;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;

                if (questUserData != null)
                    questUserData.count = count;

                if (count >= questLocalData.infoQuests[currLevel].numberRequire && !IsComplete())
                    SetCompleteQuest();
            }
        }

        private void SetCompleteQuest()
        {
            if (questUserData != null)
                questUserData.complete = 1;
            onQuestComplete?.Invoke();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
        }

        public void InitData(QuestBase questDataBase)
        {
            this.currLevel = 0;
            this.questId = questDataBase.questId;
            this.questType = questDataBase.questType;
            this.targetType = questDataBase.targetType;
            this.limitReceive = questDataBase.infoQuests[currLevel].limitReceive;
            this.rewards = questDataBase.infoQuests[currLevel].rewards;
            this.questLocalData = questDataBase;
            this.count = 0;
            this.victim = questDataBase.infoQuests[currLevel].victim;
        }

        public virtual void Init()
        {
            CleanEventQuest();
        }

        public virtual void InsertData(QuestUserData userData)
        {
            count = userData.count;
            currLevel = userData.level;
            questUserData = userData;
        }

        public void SetClaimed()
        {
            if (questUserData != null)
            {
                if (questLocalData.infoQuests.Length <= 1)
                {
                    questUserData.received = 1;
                }
                else
                {
                    if (currLevel >= (questLocalData.infoQuests.Length - 1))
                    {
                        questUserData.received = 1;
                    }
                    else
                    {
                        questUserData.level += 1;
                        currLevel += 1;
                        if (count >= questLocalData.infoQuests[currLevel].numberRequire)
                            questUserData.complete = 1;
                        else
                            questUserData.complete = 0;
                    }
                }

                UserData.Instance.Save();
            }
        }

        public virtual void CleanEventQuest()
        {
        }

        public bool IsComplete()
        {
            if (questUserData != null)
                return questUserData.complete > 0;

            return false;
        }

        public bool IsReceived()
        {
            if (this.questUserData != null)
                return questUserData.received > 0;

            return false;
        }

        public bool IsCanReceive()
        {
            return IsComplete() && !IsReceived();
        }

        public bool IsCanIncrease()
        {
            if (questLocalData.infoQuests.Length <= 1)
            {
                return !IsComplete();
            }
            else
            {
                if (!IsComplete())
                {
                    return true;
                }
                else if (IsComplete() && currLevel >= (questLocalData.infoQuests.Length - 1))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public virtual void GoToTarget()
        {
            if (UserData.Instance.CampaignData.HighestResultLevel() < this.questLocalData.infoQuests[this.currLevel].stageUnlock)
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition, this.questLocalData.infoQuests[this.currLevel].stageUnlock + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }
            
        }
    }
}