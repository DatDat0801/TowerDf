using TigerForge;

namespace EW2
{
    public class SpendQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.SpendSliverStar || targetType == TargetType.SpendGoldStar)
            {
                EventManager.StartListening(GamePlayEvent.OnSpentStarCampaign, OnSpentStarCampaign);
            }
            else if (targetType == TargetType.Crystal || targetType == TargetType.Gem)
            {
                EventManager.StartListening(GamePlayEvent.OnSpendResource, OnSpendResource);
            }
        }

        private void OnSpendResource()
        {
            var data = EventManager.GetData<int[]>(GamePlayEvent.OnSpendResource);

            if (targetType == TargetType.Crystal && data[0] == MoneyType.Crystal)
            {
                if (IsCanIncrease())
                {
                    Count += data[1];
                }
            }
            else if (targetType == TargetType.Gem && data[0] == MoneyType.Diamond)
            {
                if (IsCanIncrease())
                {
                    Count += data[1];
                }
            }
        }

        private void OnSpentStarCampaign()
        {
            var starId = EventManager.GetInt(GamePlayEvent.OnSpentStarCampaign);
            if (targetType == TargetType.SpendSliverStar && starId == MoneyType.SliverStar)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.SpendGoldStar && starId == MoneyType.GoldStar)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.SpendSliverStar || targetType == TargetType.SpendGoldStar)
            {
                EventManager.StopListening(GamePlayEvent.OnSpentStarCampaign, OnSpentStarCampaign);
            }
            else if (targetType == TargetType.Crystal || targetType == TargetType.Gem)
            {
                EventManager.StopListening(GamePlayEvent.OnSpendResource, OnSpendResource);
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.SpendSliverStar || targetType == TargetType.SpendGoldStar)
            {
                DirectionGoTo.GotoUpgradeTower();
            }
        }
    }
}