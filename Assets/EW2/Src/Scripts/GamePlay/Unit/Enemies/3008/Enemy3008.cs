using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EW2
{
    public class Enemy3008 : EnemyRanger
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public Transform enemySpawnTransform;
        public Transform bulletSpawnPos;
        public Transform attackMeleePos;
        
        private List<ActionState> m_SkillQueue;

        private List<ActionState> SkillQueue
        {
            get
            {
                if (m_SkillQueue == null) return new List<ActionState>();
                return m_SkillQueue;
            }
        }
        private Skill1Enemy3008 m_Skill1;

        public Skill1Enemy3008 Skill1
        {
            get
            {
                if (m_Skill1 == null) m_Skill1 = new Skill1Enemy3008();
                return m_Skill1;
            }
        }
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3008>().GetStats(Level);
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


        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3008Spine(this));
        
        protected override void InitAction()
        {
            base.InitAction();
            Skill1.Init(this);
        }
        // public override void AttackRange()
        // {
        //     base.AttackRange();
        //     attackRange.onComplete = () =>
        //     {
        //         if (IsValidTarget())
        //         {
        //             if (!blockCollider.CanTakeBlock() && DistanceToTarget() < EnemyData.detectMeleeAttack)
        //             {
        //                 //Debug.LogAssertion("MELEE ATTACK");
        //                 UnitSpine.AttackMelee();
        //                 AttackMelee();
        //                 
        //             }
        //         }
        //         else
        //         {
        //             ResetTarget();
        //         }
        //     };
        // }
        async void ExecuteSkill1()
        {
            if (SkillQueue.Count > 0)
            {
                if (!SkillQueue.Contains(ActionState.Skill1))
                {
                    return;
                }
            }

            UnitState.Set(ActionState.Idle);
            UnitSpine.SkillAttack();
            Skill1.CastSkill();
            await UniTask.Delay(2000);
            UnitState.Set(ActionState.Move);
            UnitSpine.Move();

            if (SkillQueue.Count > 0) SkillQueue.Remove(ActionState.Skill1);
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (Skill1.CanCastSkill())
            {
                SkillQueue.Add(ActionState.Skill1);
                ExecuteSkill1();
            }
            
        }
        //range bullet
        public async void SpawnBullet()
        {
            if (target != null)
            {
                if (target.UnitType == UnitType.Tower)
                    return;
                // var skeleton = GetComponent<SkeletonAnimation>();
                //var myBone = skeleton.Skeleton.FindBone("skull");
                //Debug.LogAssertion("Spawned BULLET");
                // GameObject bullet = ResourceUtils.GetUnit("3016_attack_range_bullet", bulletSpawnPos.position,
                //     bulletSpawnPos.rotation);
                var bullet = ResourceUtils.GetVfx("Enemy", "3008_attack_range_bullet", bulletSpawnPos.position,
                    bulletSpawnPos.rotation);

                BulletRange3016 control = bullet.GetComponent<BulletRange3016>();
                control.InitBullet(this, target, 0.5f);
                
                // Debug.LogAssertion(target.name + "  " + target.transform.position);
                // Debug.LogAssertion("target parent: " + target.transform.parent);
                //var current = UnitState.Current;
                //UnitState.Set(ActionState.AttackRange);
                //await UniTask.Delay(500);
                //UnitState.Set(current);
            }
        }
    }
}