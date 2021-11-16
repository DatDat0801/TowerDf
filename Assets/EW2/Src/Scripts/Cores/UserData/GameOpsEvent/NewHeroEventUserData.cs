using System.Collections.Generic;
using System.Linq;
using EW2.Events;

namespace EW2
{
    public class NewHeroEventUserData : BaseEventUserData
    {
        public Dictionary<int, QuestUserData> dictQuestData = new Dictionary<int, QuestUserData>();
        public int numberBuyCrystalPack;
        public int numberBuyRunePack;
        public int numberBuySpellPack;
        
        private List<QuestUserData> _listDatabase  = new List<QuestUserData>();
        
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
        
        public override bool CheckCanShow()
        {
            var newHeroEventCondition = GameContainer.Instance.Get<EventDatabase>().Get<NewHeroEventCondition>();

            if (!isOpen)
            {
                if (this.timeEndEvent <= 0)
                {
                    isOpen = true;
                    var timeEndCaculate = TimeManager.NowInSeconds + newHeroEventCondition.eventConditions[0].duration;
                    SetTimeEnd(timeEndCaculate);
                    UserData.Instance.Save();
                    return true;
                }

                return false;
            }
            else
            {
                var passCondition =
                    UserData.Instance.CampaignData.IsUnlockedStage(0,
                        newHeroEventCondition.eventConditions[0].mapUnlock);

                if (TimeRemain() > 0 && passCondition)
                {
                    return true;
                }

                isOpen = false;
                UserData.Instance.Save();
                return false;
            }
        }
    }
}