using TigerForge;

namespace EW2
{
    public class GachaQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();

            if (targetType == TargetType.GachaSpell)
            {
                EventManager.StartListening(GamePlayEvent.OnGachaSpell, OnGachaSpell);
            }
        }

        private void OnGachaSpell()
        {
            var numberGacha = EventManager.GetInt(GamePlayEvent.OnGachaSpell);
            if (IsCanIncrease())
            {
                Count += numberGacha;
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.GachaSpell)
            {
                EventManager.StopListening(GamePlayEvent.OnGachaSpell, OnGachaSpell);
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.GachaSpell)
            {
                DirectionGoTo.GotoGacha( GachaTabId.Spell);
            }
        }
    }
}