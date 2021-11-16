using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3017 : SkillEnemy
    {
        [System.NonSerialized] public EnemyData3017.EnemyData3017Skill1 skill1Data;

        [SerializeField] [SpineAnimation()] private string skill1AnimationName;
        [SerializeField] private int numberBullet;
        [SerializeField] private EnemyTargetCollection enemyTargetCollection;
        [SerializeField] private string warningVfxName;
        [SerializeField] private string bulletVfxName;
        [SerializeField] private string explosionName;
        [SerializeField] private float secondDelaySpawnBullet;
        [SerializeField] private float secondDelaySpawnEachWarning;
        [SerializeField] private float secondDelaySpawnWarning;
        [SerializeField] private float addedYCoordinateBulletPosition;


        private Enemy3017 enemy;
        private List<Dummy> calculatedTargets = new List<Dummy>();
        private List<Vector3> anyWarningPositions = new List<Vector3>();
        private List<Dummy> anyTargets;


        private AnimationEventUnit animationEventUnit = new AnimationEventUnit();
        private Vector3 currentWarningPosition;


        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            enemy = (Enemy3017) enemyBase;
            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3017>()
                .GetSkill1(enemyBase.Level);

            timeCooldown = skill1Data.cooldown;
            animationEventUnit.CompleteAnimation = CompleteAnimation;
            animationEventUnit.InitAnimationEvents(enemy.UnitSpine.SkeletonAnimation);
        }

        public override void CastSkill()
        {
            base.CastSkill();
            anyWarningPositions.Clear();
            ExecuteSkillAnimation();
            enemy.UnitState.Set(ActionState.Skill1);
            ComputeTargets();
            enemy.StartCoroutine(SpawnWarnings());
            enemy.StartCoroutine(CoSpawnBullets());
        }

        public void ComputeTargets()
        {
            calculatedTargets.Clear();
            anyTargets = CalculateAllTarget();
            ComputeTargetWhenEnemyGreaterNumberBullet();
            ComputeTargetWhenEnemyLesserNumberBullet();
        }

        public override bool CanCastSkill()
        {
            if (base.CanCastSkill())
            {
                if (CalculateAllTarget().Count != 0)
                    return true;
            }

            return false;
        }

        public StunStatus CalculateStunStatus(Unit target)
        {
            var stunStatus = new StunStatus(new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = target,
                lifeTime = skill1Data.secondApply,
                chanceApply = skill1Data.percentChanceApply
            });
            return stunStatus;
        }

        private List<Dummy> CalculateAllTarget()
        {
            var anyTargets = new List<Dummy>();
            foreach (var target in enemyTargetCollection.CalculateAllTarget())
            {
                if (target.IsAlive && target.gameObject.activeInHierarchy)
                {
                    anyTargets.Add(target);
                }
            }

            return anyTargets;
        }

        private void ExecuteSkillAnimation()
        {
            enemy.enemySpine.AnimationState.SetAnimation(0, skill1AnimationName, false);
        }

        private void ComputeTargetWhenEnemyLesserNumberBullet()
        {
            if (anyTargets.Count < numberBullet)
            {
                var index = 0;
                while (calculatedTargets.Count < numberBullet)
                {
                    calculatedTargets.Add(anyTargets[index]);
                    index++;
                    if (index == anyTargets.Count)
                    {
                        index = 0;
                    }
                }
            }
        }

        private void ComputeTargetWhenEnemyGreaterNumberBullet()
        {
            if (anyTargets.Count >= numberBullet)
            {
                foreach (var target in anyTargets)
                {
                    if (IsHero(target))
                    {
                        calculatedTargets.Add(target);
                    }
                }

                if (calculatedTargets.Count < numberBullet)
                {
                    for (int i = 0, length = anyTargets.Count; i < length; i++)
                    {
                        if (!calculatedTargets.Contains(anyTargets[i]))
                        {
                            calculatedTargets.Add(anyTargets[i]);
                        }

                        if (calculatedTargets.Count == numberBullet)
                        {
                            break;
                        }
                    }
                }
            }
        }


        private void SpawnExplosions()
        {
            var explosionClone = ResourceUtils.GetVfx(EffectType.Enemy.ToString(), explosionName,
                currentWarningPosition,
                Quaternion.identity,null,3).GetComponentInChildren<EnemyNormalDamageBox>();
            explosionClone.SetOwner(enemy);
        }

        private IEnumerator CoSpawnBullets()
        {
            yield return new WaitForSeconds(secondDelaySpawnBullet);
            for (int i = 0, length = calculatedTargets.Count; i < length; i++)
            {
                currentWarningPosition = anyWarningPositions[i];
                var bulletSpawnPosition = new Vector3(currentWarningPosition.x,
                    currentWarningPosition.y + addedYCoordinateBulletPosition, currentWarningPosition.z);
                var bulletClone = ResourceUtils.GetVfx(EffectType.Enemy.ToString(), bulletVfxName, bulletSpawnPosition,
                    Quaternion.identity,null,3).GetComponent<BulletSkill1Enemy3017>();
                bulletClone.TargetPosition = currentWarningPosition;
                bulletClone.FinishMove = SpawnExplosions;
                yield return new WaitForSeconds(secondDelaySpawnEachWarning);
            }
        }

        private IEnumerator SpawnWarnings()
        {
            yield return new WaitForSeconds(secondDelaySpawnWarning);
            foreach (var target in calculatedTargets)
            {
                anyWarningPositions.Add(target.transform.position);
                ResourceUtils.GetVfx(EffectType.Enemy.ToString(), warningVfxName, target.transform.position,
                    Quaternion.identity,null,3);
                yield return new WaitForSeconds(secondDelaySpawnEachWarning);
            }
        }


        private bool IsHero(Dummy dummy) => dummy is HeroBase;

        private void CompleteAnimation(TrackEntry trackEntry)
        {
            var animationName = trackEntry.Animation.Name;
            if (animationName == skill1AnimationName)
            {
                enemy.UnitState.Set(ActionState.Idle);
            }
        }
    }
}