using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Soldier2001 : SoldierBase
    {
        public Transform pointSpawnBullet;

        public Tower2001 Tower { get; protected set; }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Soldier2001Spine(this));

        public override RPGStatCollection Stats
        {
            get
            {
                var towerData2001 = Tower.TowerData as TowerData2001;
                if (towerData2001 != null)
                    stats = new Tower2001Stats(this, towerData2001.GetDataStatBaseByLevel(Tower.Level));

                var attackSpeed = stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);
                attackSpeed.StatBaseValue =
                    SpineUtils.GetAnimationTime(this.UnitSpine.AnimationState, this.UnitSpine.attackRangeName);

                return stats;
            }
        }

        private AttackRange attackRange;
        private Unit targetEnemy;
        private List<EnemyBase> targets;

        private bool useSkill;
        private TowerData2001.Skill1 dataSkill;
        private Action callbackAtkSkillComplete;
        private Action callbackAtkNormalComplete;
        private Coroutine delaySpawnBulletSkill;

        public override void OnUpdate(float deltaTime)
        {
            switch (UnitState.Current)
            {
                case ActionState.None:
                    idle.Execute();
                    break;
                case ActionState.AttackRange:
                    if (targetEnemy == null) return;
                    Flip(targetEnemy.transform.position.x);
                    attackRange.Execute(deltaTime);
                    break;
            }
        }

        public void InitDataSoldier(int soldierId, Tower2001 owner)
        {
            this.owner = Tower = owner;

            this.idSoldier = soldierId;

            DamageType = owner.DamageType;

            SetId(owner.Id);

            InitAction();

            Tower.Level.ValueChanged += OnTowerLevelUp;
        }

        private void OnTowerLevelUp(object sender, EventArgs e)
        {
            ((TowerStats)Stats).UpdateStats(((TowerData2001)Tower.TowerData).GetDataStatBaseByLevel(Tower.Level));
        }

        protected override void InitAction()
        {
            base.InitAction();

            this.attackRange = new AttackRange(this);
        }

        #region Action

        public override bool IsAlive => true;

        public override void Remove()
        {
            if (delaySpawnBulletSkill != null)
                CoroutineUtils.Instance.StopCoroutine(delaySpawnBulletSkill);
        }

        public override void AttackRange()
        {
            var idSoundRandom = Random.Range(1, 10);

            attackRange.onComplete = () =>
            {
                var soundName = string.Format(SoundConstant.Tower2001Cast, idSoundRandom);
                EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(this.Tower.Id, soundName),
                    EazySoundManager.GlobalSoundsVolume);
                UnitState.Set(ActionState.None);
                attackRange.ResetTime();
                callbackAtkNormalComplete?.Invoke();
                callbackAtkNormalComplete = null;
            };
            UnitState.Set(ActionState.AttackRange);
        }


        public void Idle()
        {
            //this.targetEnemy = null;
            useSkill = false;
            attackRange.ResetTime();
            UnitState.Set(ActionState.None);
        }

        public void AttackTarget(Unit target, List<EnemyBase> listTargets, Action completeAtk)
        {
            targetEnemy = target;
            targets = listTargets;
            if (targetEnemy == null)
            {
                UnitState.Set(ActionState.None);
                return;
            }

            callbackAtkNormalComplete = completeAtk;
            AttackRange();
        }
        public void UpdateTarget(List<EnemyBase> listTargets)
        {
            targets = listTargets;
        }
        public void AttackSkill(Unit target, TowerData2001.Skill1 dataSkill, Action completeAtk)
        {
            useSkill = true;
            this.dataSkill = dataSkill;
            targetEnemy = target;

            callbackAtkSkillComplete = completeAtk;
            attackRange.ResetTime();
            attackRange.onComplete = () =>
            {
                UnitState.Set(ActionState.None);
                attackRange.ResetTime();
            };
            UnitState.Set(ActionState.AttackRange);
        }

        #endregion

        public void SpawnBullet()
        {
            if (targetEnemy != null)
            {
                //GameObject bullet;

                if (!useSkill)
                {
                    for (var i = 0; i < targets.Count; i++)
                    {
                        var bullet = ResourceUtils.GetUnit("bullet_2001", pointSpawnBullet.position,
                            pointSpawnBullet.rotation);

                        Bullet2001 control = bullet.GetComponent<Bullet2001>();
                        control.InitBullet2001(this, targets[i], 0.5f);
                    }
                }
                else
                {
                    if (delaySpawnBulletSkill != null)
                        CoroutineUtils.Instance.StopCoroutine(delaySpawnBulletSkill);
                    delaySpawnBulletSkill = CoroutineUtils.Instance.StartCoroutine(SpawnBulletEffect());
                }
            }
        }

        IEnumerator SpawnBulletEffect()
        {
            var posEnd = targetEnemy.transform.position;

            useSkill = false;
            callbackAtkSkillComplete?.Invoke();
            callbackAtkSkillComplete = null;

            for (int i = 0; i < dataSkill.numberArrow; i++)
            {
                if (targets != null)
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        var bullet = ResourceUtils.GetUnit("bullet_2001_skill", pointSpawnBullet.position,
                            pointSpawnBullet.rotation);
                        Bullet2001Skill1 control = bullet.GetComponent<Bullet2001Skill1>();
                        if (targets[j] != null)
                        {
                            //Add more damage based on user persistent upgraded system
                            var damage = dataSkill.damage * (1 + Tower.towerData.BonusStat.bonusDamagePerArrowSkill1);
                            control.InitBulletSkill2001(this, targets[j], 0.5f, damage);
                        }
                        else
                        {
                            control.InitBulletSkill2001(posEnd, 0.5f);
                        }

                        EazySoundManager.PlaySound(
                            ResourceSoundManager.GetSoundTower(this.Tower.Id, SoundConstant.Tower2001CastSkill1),
                            EazySoundManager.GlobalSoundsVolume);
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForSeconds(dataSkill.delayAttack);
            }

        }
    }
}
