using UnityEngine;

namespace EW2
{
    public class Enemy3002 : EnemyRanger
    {
        public EnemyNormalDamageBox normalAttackBox;

        public Transform pointSpawnBullet;

        [SerializeField] private float rangeAttackMelee;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3002>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3002Spine(this));

        protected override void InitAction()
        {
            base.InitAction();

            attackMelee.Range = rangeAttackMelee;
        }

        public override void AttackRange()
        {
            attackRange.onComplete = () =>
            {
                if(IsValidTarget())
                {
                    if (!blockCollider.CanTakeBlock() && DistanceToTarget() < rangeAttackMelee)
                    {
                        AttackMelee();
                    }
                }
                else
                {
                    ResetTarget();
                }
            };

            base.AttackRange();
        }

        public void SpawnBullet()
        {
            if (target != null)
            {
                GameObject bullet =
                    ResourceUtils.GetUnit("bullet_3002", pointSpawnBullet.position,
                        pointSpawnBullet.rotation);
                if (bullet != null)
                {
                    Bullet3002 control = bullet.GetComponent<Bullet3002>();
                    control.InitBullet(this, target, 0.5f);
                }
            }
        }
    }
}