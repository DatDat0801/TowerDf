using UnityEngine;

namespace EW2
{
    public class Enemy3003 : EnemyRanger
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
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3003>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3003Spine(this));

        public override void AttackRange()
        {
            attackRange.onComplete = () =>
            {
                if(IsValidTarget())
                {
                    // Debug.LogWarning($"Distance= " + DistanceToTarget());

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
                    ResourceUtils.GetUnit("bullet_3003", pointSpawnBullet.position,
                        pointSpawnBullet.rotation);
                if (bullet != null)
                {
                    var control = bullet.GetComponent<Bullet3003>();
                    control.InitBullet(this, target, 0.5f);
                }
            }
        }
    }
}