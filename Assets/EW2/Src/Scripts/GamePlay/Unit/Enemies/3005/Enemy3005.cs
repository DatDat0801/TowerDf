using UnityEngine;

namespace EW2
{
    public class Enemy3005 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public EnemyNormalDamageBox normalAttackBox2;
        public EnemyNormalDamageBox normalAttackBox3;

        [SerializeField] private Skill1Enemy3005 skill1;
        
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3005>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3005Spine(this));

        public bool IsPhase1()
        {
            var health = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            return skill1.GetPercentHpCanApply()<= health.CalculateCurrentPercent();
        }

        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);
            skill1.CastSkill();
        }

        private void InitSkills()
        {
            skill1.Init(this);
        }
        
      
     
    }
}