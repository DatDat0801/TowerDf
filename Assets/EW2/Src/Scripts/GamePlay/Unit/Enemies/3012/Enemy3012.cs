using System.Collections;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Enemy3012 : EnemyBase
    {
        [SerializeField] private Transform pointSpawn;
        public EnemyNormalDamageBox normalAttackBox1;
        public EnemyNormalDamageBox normalAttackBox2;

        private Skill1Enemy3012 skill1;

        public Skill1Enemy3012 Skill1
        {
            get
            {
                if (skill1 == null)
                    skill1 = new Skill1Enemy3012();
                return skill1;
            }
        }

        private const int EnemyIdSpawn = 3004;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3012>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3012Spine(this));

        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }

        private void InitSkills()
        {
            Skill1.Init(this);
        }
        
        public override void Remove()
        {
            base.Remove();
            die.onComplete = () => { LeanPool.Despawn(gameObject); };
        }

        public void SpawnEnemies()
        {
            if (IsCompleteEndPoint && !StatusController.CanUseSkill()) return;

            var paths = LineController.CalculateRemainPathWayPoints(transform.position);

            var goEnemy =
                GamePlayController.Instance.SpawnController.SpawnEnemy(EnemyIdSpawn, pointSpawn.position, paths);

            if (goEnemy)
                goEnemy.EnableColliderImmediate();
        }
    }
}