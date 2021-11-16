using TigerForge;

namespace EW2
{
    public class SummonQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.SummonSpell || targetType == TargetType.SummonRune || targetType == TargetType.SummonRunePremium)
            {
                EventManager.StartListening(GamePlayEvent.OnSummon, OnSummom);
            }

            if (targetType == TargetType.AnyHero)
                EventManager.StartListening(GamePlayEvent.OnHeroUnlocked, OnUnlockHero);
        }

        private void OnUnlockHero()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnSummom()
        {
            var dataSummon = EventManager.GetData<int[]>(GamePlayEvent.OnSummon);

            if (targetType == TargetType.SummonSpell && dataSummon[0] == InventoryType.Spell)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.SummonRune && dataSummon[0] == InventoryType.Rune)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.SummonRunePremium && dataSummon[0] == InventoryType.Rune && dataSummon[1] == (int)SummonType.Premium)
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
            if (targetType == TargetType.SummonSpell || targetType == TargetType.SummonRune || targetType == TargetType.SummonRunePremium)
            {
                EventManager.StopListening(GamePlayEvent.OnSummon, OnSummom);
            }

            if (targetType == TargetType.AnyHero)
                EventManager.StopListening(GamePlayEvent.OnHeroUnlocked, OnUnlockHero);
        }

        public override void InsertData(QuestUserData userData)
        {
            base.InsertData(userData);
            CheckHeroUnlockedStartGame();
        }

        private void CheckHeroUnlockedStartGame()
        {
            if (targetType == TargetType.AnyHero)
            {
                if (Count == 0)
                {
                    Count = UserData.Instance.UserHeroData.NumberHeroUnlocked();
                }
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.SummonSpell)
            {
                DirectionGoTo.GotoGacha( GachaTabId.Spell);
            }else if ( targetType == TargetType.SummonRune || targetType == TargetType.SummonRunePremium)
            {
                DirectionGoTo.GotoGacha( GachaTabId.Rune);
            }
            else if (targetType == TargetType.AnyHero)
            {
                DirectionGoTo.GotoHeroRoom();
            }
        }
    }
}