using TigerForge;

namespace EW2
{
    public class KillByQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.ArcherKill || targetType == TargetType.BarrackKill ||
                targetType == TargetType.MagicKill || targetType == TargetType.GolemKill)
            {
                EventManager.StartListening(GamePlayEvent.OnEnemyKill, OnEnemyKill);
            }
        }

        private void OnEnemyKill()
        {
            var data = EventManager.GetData<int[]>(GamePlayEvent.OnEnemyKill);
            if (data != null && data.Length > 0)
            {
                var idCreator = data[1];

                if (targetType == TargetType.ArcherKill && idCreator == 2001)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
                else if (targetType == TargetType.BarrackKill && idCreator == 2004)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
                else if (targetType == TargetType.MagicKill && idCreator == 2002)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
                else if (targetType == TargetType.GolemKill && idCreator == 2003)
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.ArcherKill || targetType == TargetType.BarrackKill ||
                targetType == TargetType.MagicKill || targetType == TargetType.GolemKill)
            {
                EventManager.StopListening(GamePlayEvent.OnEnemyKill, OnEnemyKill);
            }
        }


        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.ArcherKill || targetType == TargetType.BarrackKill ||
                targetType == TargetType.MagicKill || targetType == TargetType.GolemKill)
            {
                DirectionGoTo.GotoNormalCampaign();
            }
        }
    }
}