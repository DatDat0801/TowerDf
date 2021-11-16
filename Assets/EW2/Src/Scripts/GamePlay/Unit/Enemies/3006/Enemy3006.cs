using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class Enemy3006 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public EnemyNormalDamageBox normalAttackBox2;
        
        [SerializeField] private Skill1Enemy3006 skill1;
        
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3006>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3006Spine(this));

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            skill1.UpdateSkill();
        }
        
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