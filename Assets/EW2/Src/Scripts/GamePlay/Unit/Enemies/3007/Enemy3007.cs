using UnityEngine;

namespace EW2
{
    public class Enemy3007 : EnemyRanger
    {
        public EnemyNormalDamageBox normalAttackBox;

        public Transform pointSpawnBullet;

        [SerializeField] private float rangeAttackMelee;
        [SerializeField] private Skill1Enemy3007 skill1;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3007>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3007Spine(this));

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                GetHurt(new DamageInfo() {
                    creator = FindObjectOfType<Hero1001>(),
                    target = this,
                    value = 1000000,
                    damageType = DamageType.Physical
                });
            }

            if (skill1.CanCastSkill())
            {
                UnitState.Set(ActionState.UseSkill);
                skill1.CastSkill();
                return;
            }

            base.OnUpdate(deltaTime);
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

        public override void AttackRange()
        {
            attackRange.onComplete = () => {
                if (IsValidTarget())
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
                    ResourceUtils.GetUnit("3007_range_bullet", pointSpawnBullet.position,
                        pointSpawnBullet.rotation);
                if (bullet != null)
                {
                    var control = bullet.GetComponent<Bullet3007>();
                    control.Init(this, target, 2, 5);
                }
            }
        }


        public void PlaySkill()
        {
            skill1.ApplyEffectForAnyAllies();
        }
    }
}