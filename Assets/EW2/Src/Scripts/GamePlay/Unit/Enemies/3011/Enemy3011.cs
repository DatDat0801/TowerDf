using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class Enemy3011 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public EnemyNormalDamageBox normalAttackBox2;

        private Skill1Enemy3011 skill1;

        public Skill1Enemy3011 Skill1
        {
            get
            {
                if (skill1 == null)
                    skill1 = new Skill1Enemy3011();
                return skill1;
            }
        }

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3011>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3011Spine(this));


        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }

        private void InitSkills()
        {
            Skill1.Init(this);
        }
    }
}