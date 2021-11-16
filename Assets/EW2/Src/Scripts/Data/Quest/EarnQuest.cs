using TigerForge;

namespace EW2
{
    public class EarnQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.EarnStar)
            {
                EventManager.StartListening(GamePlayEvent.OnEarnStarCampaign, OnEarnStarCampaign);
            }
            else if (targetType == TargetType.NormalSpell)
            {
                EventManager.StartListening(GamePlayEvent.OnEarnSpell, OnEarnSpell);
            }
        }

        private void OnEarnSpell()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnEarnStarCampaign()
        {
            var data = EventManager.GetData<int[]>(GamePlayEvent.OnEarnStarCampaign);
            if (data != null && data.Length > 0)
            {
                var starId = data[0];
                var numberStar = data[1];

                if (starId == questLocalData.infoQuests[currLevel].victim[0])
                {
                    if (IsCanIncrease())
                    {
                        Count += numberStar;
                    }
                }
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.EarnStar)
            {
                EventManager.StopListening(GamePlayEvent.OnEarnStarCampaign, OnEarnStarCampaign);
            }
            else if (targetType == TargetType.NormalSpell)
            {
                EventManager.StopListening(GamePlayEvent.OnEarnSpell, OnEarnSpell);
            }
        }

        public override void InsertData(QuestUserData userData)
        {
            base.InsertData(userData);
            if (targetType == TargetType.NormalSpell)
            {
                if (Count == 0)
                {
                    var numbSpellEarned = UserData.Instance.GetListSpell().Count;
                    if (numbSpellEarned > 0)
                        Count = numbSpellEarned;
                }
            }
            else if (targetType == TargetType.EarnStar)
            {
                var campaignData = UserData.Instance.CampaignData;

                if (questLocalData.infoQuests[currLevel].victim[0] == MoneyType.SliverStar)
                {
                    var currSliverStar = campaignData.GetTotalStars((int)ModeCampaign.Normal);
                    if (Count != currSliverStar)
                    {
                        Count = currSliverStar;
                    }
                }
                else
                {
                    var currGoldStar = campaignData.GetTotalStars((int)ModeCampaign.Nightmare);
                    if (Count != currGoldStar)
                    {
                        Count = currGoldStar;
                    }
                }
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.EarnStar)
            {
                if (victim[0] == MoneyType.SliverStar)
                {
                    DirectionGoTo.GotoNormalCampaign();
                }
                else if (victim[0] == MoneyType.GoldStar)
                {
                    if (UserData.Instance.CampaignData.IsShowUnlockNightmareNotice(0))
                    {
                        DirectionGoTo.GotoNighmareCampaign();
                    }
                    else
                    {
                        EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                            string.Format(L.popup.spell_unlock_condition, GameConfig.MAX_STAGE_ID_WORLD_1 + 1));
                    }
                }
            }
            else if (targetType == TargetType.NormalSpell)
            {
                DirectionGoTo.GotoGacha(GachaTabId.Spell);
            }
        }
    }
}
