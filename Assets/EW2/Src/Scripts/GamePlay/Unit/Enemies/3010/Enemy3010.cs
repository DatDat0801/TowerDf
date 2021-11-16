using Lean.Pool;

namespace EW2
{
    public class Enemy3010 : EnemyBase
    {
        public Skill1Enemy3010 skill1;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3010>().GetStats(Level);
                }

                return enemyData;
            }
        }

        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }

        public override void Remove()
        {
            base.Remove();
            die.onComplete = () => { };

            if (IsCompleteEndPoint)
            {
                LeanPool.Despawn(gameObject);
                return;
            }

            if (StatusController.CanUseSkill())
            {
                CoroutineUtils.Instance.StartCoroutine(skill1.SpawnEnemies());
            }
        }

        public override void CheckEnemyDie()
        {
            if (IsCompleteEndPoint)
                base.CheckEnemyDie();
        }

        private void InitSkills()
        {
            skill1.Init(this);
        }
    }
}