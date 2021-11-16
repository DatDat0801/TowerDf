using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3006 : SkillEnemy
    {
        [SerializeField] private AllyTargetCollection allyTargetCollection;

        private EnemyData3006.EnemyData3006Skill1 skill1Data;
        private Enemy3006 enemy;

        public override void CastSkill()
        {
            base.CastSkill();
            ApplyEffectForAnyAllies();
        }

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3006>()
                .GetSkill1(enemyBase.Level);
            timeCooldown = skill1Data.cooldown;
            enemy = (Enemy3006) enemyBase;
        }

        public void UpdateSkill()
        {
            if (CanCastSkill())
            {
                CastSkill();
            }
        }

        private void ApplyEffectForAnyAllies()
        {
            allyTargetCollection.SetTargetType(MoveType.All);
            var anyAllies = allyTargetCollection.CalculateAllTarget();
            foreach (var ally in anyAllies)
            {
                if (ally == enemy || ally.Id == 3006)
                {
                    continue;
                }

                var moveSpeedAttribute = ally.Stats.GetStat<RPGAttribute>(RPGStatType.MoveSpeed);
                var statusOverTimeConfig = new StatusOverTimeConfig()
                {
                    creator = ally,
                    owner = ally,
                    lifeTime = skill1Data.secondApply,
                    intervalTime = skill1Data.secondApply,
                    statusType = StatusType.BuffMoveSpeed
                };
                var modifierMoveSpeed = new RPGStatModifier(moveSpeedAttribute, skill1Data.modifierType,
                    skill1Data.percentIncreaseMoveSpeed, false, enemy, enemy);
                var modifierMoveSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierMoveSpeed);
                ally.StatusController.AddStatus(modifierMoveSpeedOverTime);
            }
        }
    }
}