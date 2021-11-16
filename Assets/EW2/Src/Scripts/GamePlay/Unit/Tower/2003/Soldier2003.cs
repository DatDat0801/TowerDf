using System;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Soldier2003 : SoldierBase
    {
        public Transform pointSpawnBullet;

        public Transform pointSpawnSkill1;

        private Tower2003 Tower { get; set; }

        private TowerData2003.Skill2 dataSkill2;

        private TowerData2003.Skill1 dataSkill1;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Soldier2003Spine(this));

        public override RPGStatCollection Stats
        {
            get
            {
                var towerData2003 = Tower.TowerData as TowerData2003;
                if (towerData2003 != null && stats == null)
                    stats = new Tower2003Stats(this, towerData2003.GetDataStatBaseByLevel(Tower.Level));

                return stats;
            }
        }

        private AttackRange attackRange;
        private SkillPassive1 skillPassive1;
        private SkillPassive2 skillPassive2;
        private Unit targetEnemy;
        private float damageSkill;
        private Action callbackAtkComplete;

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

        public void InitDataSoldier(int soldierId, Tower2003 owner)
        {
            this.owner = this.Tower = owner;

            this.idSoldier = soldierId;

            DamageType = owner.DamageType;

            SetId(owner.Id);

            InitAction();

            Tower.Level.ValueChanged += OnTowerLevelUp;

            Tower.Stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed).PropertyChanged += (sender, args) =>
            {
                var attackSpeed = stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);
                attackSpeed.StatBaseValue = Tower.Stats.GetStat(RPGStatType.AttackSpeed).StatValue;
            };
        }

        private void OnTowerLevelUp(object sender, EventArgs e)
        {
            ((TowerStats) Stats).UpdateStats(((TowerData2003) Tower.TowerData).GetDataStatBaseByLevel(Tower.Level));
        }

        protected override void InitAction()
        {
            base.InitAction();

            this.attackRange = new AttackRange(this);
            this.skillPassive1 = new SkillPassive1(this)
            {
                onComplete = () =>
                {
                    UnitState.Set(ActionState.None);
                    attackRange.ResetTime();
                }
            };
            this.skillPassive2 = new SkillPassive2(this)
            {
                onComplete = () =>
                {
                    UnitState.Set(ActionState.None);
                    attackRange.ResetTime();
                }
            };
        }

        #region Action

        public override bool IsAlive => true;

        public override void Remove()
        {
        }

        public override void AttackRange()
        {
            attackRange.onComplete = () =>
            {
                UnitState.Set(ActionState.None);
                attackRange.ResetTime();
                callbackAtkComplete?.Invoke();
            };

            UnitState.Set(ActionState.AttackRange);
        }


        public void Idle()
        {
            this.targetEnemy = null;

            attackRange.ResetTime();

            UnitState.Set(ActionState.None);
        }

        public void AttackTarget(Unit target, Action completeAtk)
        {
            this.targetEnemy = target;

            if (this.targetEnemy == null)
            {
                UnitState.Set(ActionState.None);
                return;
            }

            callbackAtkComplete = completeAtk;

            AttackRange();
        }

        public void AttackSkill1(TowerData2003.Skill1 data2003SkillActive, Action completeUseSkill = null)
        {
            dataSkill1 = data2003SkillActive;

            skillPassive1.onComplete = () => completeUseSkill?.Invoke();

            targetEnemy = this.Tower.SearchTarget.SelectTarget();

            Flip(targetEnemy.transform.position.x);

            skillPassive1.Execute();
        }

        public void AttackSkill2(TowerData2003.Skill2 data2003SkillActive, Action completeUseSkill = null)
        {
            dataSkill2 = data2003SkillActive;

            skillPassive2.onComplete = () => completeUseSkill?.Invoke();

            skillPassive2.Execute();
        }

        #endregion

        public void SpawnBullet()
        {
            if (targetEnemy != null)
            {
                GameObject bullet = null;

                bullet = ResourceUtils.GetUnit("bullet_2003", pointSpawnBullet.position, pointSpawnBullet.rotation);

                if (bullet != null)
                {
                    Bullet2003 control = bullet.GetComponent<Bullet2003>();

                    control.InitBullet2003(this.Tower, targetEnemy, 0.5f);
                }
            }
        }

        public void SpawnEffectSkill1()
        {
            var goEffect =
                ResourceUtils.GetVfxTower("2003_skill_1", pointSpawnSkill1.transform.position, Quaternion.identity,null,10);
            if (goEffect != null)
            {
                EazySoundManager.PlaySound(
                    ResourceSoundManager.GetSoundTower(this.Tower.Id, SoundConstant.Tower2003CastSkill),
                    EazySoundManager.GlobalSoundsVolume);

                Tower2003Skill1Impact control = goEffect.GetComponent<Tower2003Skill1Impact>();

                if (control && dataSkill1 != null)
                {
                    control.InitImpact(this.Tower, dataSkill1, targetEnemy);
                }
            }
        }

        public void SpawnEffectSkill2()
        {
            var goEffect =
                ResourceUtils.GetVfxTower("2003_skill_2", Vector3.zero, Quaternion.identity, this.owner.transform,10);
            if (goEffect != null)
            {
                EazySoundManager.PlaySound(
                    ResourceSoundManager.GetSoundTower(this.Tower.Id, SoundConstant.Tower2003CastSkill),
                    EazySoundManager.GlobalSoundsVolume);

                Tower2003Skill2Impact control = goEffect.GetComponent<Tower2003Skill2Impact>();

                if (control && dataSkill2 != null)
                {
                    control.InitImpact((Tower2003) this.owner, dataSkill2);
                }
            }
        }
    }
}