using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class Enemy3015 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public Skill1Enemy3015 skill1;
        public Skill2Enemy3015 skill2;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3015>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new EnemyStats(this, enemyData);
                }

                return stats;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3015Spine(this));

        public bool IsPhase1()
        {
            var health = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            return skill2.GetPercentHpCanApply() >= health.CalculateCurrentPercent();
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);
            skill2.CastSkill();
        }

        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }

        private void InitSkills()
        {
            skill1.Init(this);
            skill2.Init(this);
        }
    }
}