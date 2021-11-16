using TigerForge;

namespace EW2
{
    public class KillQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.KillBoss || targetType == TargetType.KillAnyEnemy)
            {
                EventManager.StartListening(GamePlayEvent.OnEnemyKill, OnEnemyKill);
            }
        }

        private void OnEnemyKill()
        {
            if (targetType == TargetType.KillAnyEnemy)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.KillBoss)
            {
                var data = EventManager.GetData<int[]>(GamePlayEvent.OnEnemyKill);
                if (data != null && data.Length > 0)
                {
                    var enemyId = data[0];

                    if (enemyId == questLocalData.infoQuests[currLevel].victim[0])
                    {
                        if (IsCanIncrease())
                        {
                            Count++;
                        }
                    }
                }
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.KillBoss || targetType == TargetType.KillAnyEnemy)
            {
                EventManager.StopListening(GamePlayEvent.OnEnemyKill, OnEnemyKill);
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();
            
            if (targetType == TargetType.KillBoss)
            {
                DirectionGoTo.GotoMapHaveBoss(victim[0]);
            }
            else if (targetType == TargetType.KillAnyEnemy)
            {
                DirectionGoTo.GotoNormalCampaign();
            }
        }
    }
}