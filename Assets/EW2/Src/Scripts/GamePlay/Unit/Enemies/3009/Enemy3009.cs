using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class Enemy3009 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public Skill1Enemy3009 skill1;
        
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3009>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3009Spine(this));

       
        
        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }
        
        private void InitSkills()
        {
            skill1.Init(this);
        }
        
    }
}