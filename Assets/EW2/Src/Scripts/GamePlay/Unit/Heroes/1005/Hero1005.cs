using System;
using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;

namespace EW2
{
    public class Hero1005 : HeroBase
    {
        public HeroNormalDamageBox normalAttackBox;
        public Transform pointSpawnBullet;
        public Transform activeSpawnBullet;
        public Collider2D bodyHero;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Hero1005Spine(this));

        public override HeroData HeroData
        {
            get
            {
                if (heroData == null)
                {
                    heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1005>();
                }

                return heroData;
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));
        private AoeCountTime _aoeCountTime;

        public AoeCountTime AoeCountTime
        {
            get
            {
                if (this._aoeCountTime == null)
                {
                    this._aoeCountTime =
                        new AoeCountTime() { Passive2Data = this.PassiveSkill2.SkillData, Hero1005 = this };
                }

                return this._aoeCountTime;
            }
        }

        public Hero1005ActiveSkill ActiveSkill { get; private set; }
        public Hero1005PassiveSkill1 PassiveSkill1 { get; private set; }
        public Hero1005PassiveSkill2 PassiveSkill2 { get; private set; }
        private Hero1005PassiveSkill3 passiveSkill3;
        private Reborn _reborn;

        /// <summary>
        /// add by kill enemy use active skill
        /// </summary>
        //public float KilledEnemyByActiveSkill { get; set; }

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            SetInfo(1005);

            SetRangeAttack();
            attackRange = new AttackRange(this);
            InitSkill();
            SearchTarget.SetBlockNumber(HeroStatBase.blockEnemy);
            //attackRange.onComplete = CheckActivePassive;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitCustomStats();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        #endregion

        protected override void InitAction()
        {
            base.InitAction();
            this._reborn = new Reborn(this);
        }

        public void InitSkill()
        {
            var data = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1005>();

            ActiveSkill = new Hero1005ActiveSkill(this, data.active);
            ActiveSkill.Init();
            SkillController.SetSkillActive(ActiveSkill);

            this.PassiveSkill1 = new Hero1005PassiveSkill1(this, data.passive1);
            this.PassiveSkill1.Init();
            SkillController.AddSkillPassive(this.PassiveSkill1);

            this.PassiveSkill2 = new Hero1005PassiveSkill2(this, data.passive2);
            this.PassiveSkill2.Init();
            SkillController.AddSkillPassive(this.PassiveSkill2);

            passiveSkill3 = new Hero1005PassiveSkill3(this, data.passive3);
            passiveSkill3.Init();
            SkillController.AddSkillPassive(passiveSkill3);
        }

        void InitCustomStats()
        {
            //add custom stat
            if (Stats is HeroStats heroStats)
            {
                var skillData = ((Hero1005ActiveSkill)SkillController.SkillActive).SkillData;
                if (skillData != null)
                    heroStats.AddCustomCooldownStat(skillData.cooldown);
            }
        }

        public override float GetCoolDownTime()
        {
            var timeCooldown = ((Hero1005ActiveSkill)SkillController.SkillActive).SkillData.cooldown;
            timeCooldown -= timeCooldown * Stats.GetStat(RPGStatType.CooldownReduction).StatValue;

            //bonus killed enemy by active skill
            // timeCooldown -= KilledEnemyByActiveSkill * ActiveSkill.SkillData.cooldownDecrease;
            // KilledEnemyByActiveSkill = 0;
            return timeCooldown;
        }

        private Vector3 _targetActiveSkill;

        public override void ActiveSkillToTarget(Vector3 target, Action callbackCooldown)
        {
            base.ActiveSkillToTarget(target, callbackCooldown);

            if (!CanControl)
                return;

            move.Stop();
            UnitState.Set(ActionState.None);
            this._targetActiveSkill = target;
            UseSkillActive();
            if (callbackCooldown != null)
                callbackCooldown.Invoke();
        }

        public override void UseSkillActive()
        {
            if (!CanControl)
                return;

            SkillController.RunActiveSkill();


            skillAttack.onComplete = () => {
                UnitState.IsLockState = false;
                UnitState.Set(ActionState.None);
                CheckActionCallback();
            };

            Flip(_targetActiveSkill.x);

            skillAttack.Execute();

            UnitState.IsLockState = true;

            base.UseSkillActive();
        }

        public void SpawnBullet()
        {
            switch (UnitState.Current)
            {
                //case ActionState.SkillPassive1:
                case ActionState.AttackRange:
                    if (SearchTarget.HasTarget)
                    {
                        var target = SearchTarget.target.Value;
                        if (target != null)
                        {
                            GameObject bullet =
                                ResourceUtils.GetUnit("1005_attack_range_ball", pointSpawnBullet.position,
                                    pointSpawnBullet.rotation);
                            if (bullet != null)
                            {
                                BulletRange1005 control = bullet.GetComponent<BulletRange1005>();
                                //control.InitBullet1002(this, target, 0.45f);
                                control.InitBullet1005(this, target, 0.4f);
                                var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1005_RANGE);
                                EazySoundManager.PlaySound(audioClip1);
                            }
                        }
                        else
                        {
                            Debug.LogAssertion("target is null");
                        }
                    }

                    break;
                case ActionState.UseSkill:

                    GameObject bulletSkillActive =
                        ResourceUtils.GetVfxHero("1005_active_ball", this.activeSpawnBullet.position,
                            this.activeSpawnBullet.rotation);
                    if (bulletSkillActive != null)
                    {
                        BulletActive1005 control = bulletSkillActive.GetComponent<BulletActive1005>();
                        control.InitBullet1005ActiveSkill(this, _targetActiveSkill, 1f);
                    }

                    break;
            }
        }

        private HashSet<EnemyBase> _enemiesGetDamage;

        /// <summary>
        /// Track enemy get damage by 1005
        /// </summary>
        public HashSet<EnemyBase> EnemiesGetDamage
        {
            get
            {
                if (this._enemiesGetDamage == null)
                {
                    this._enemiesGetDamage = new HashSet<EnemyBase>();
                }

                return this._enemiesGetDamage;
            }
        }

        public void AddEnemyToTrack(EnemyBase enemyBase)
        {
            if (PassiveSkill1.Level > 0)
            {
                //if(EnemiesGetDamage.Contains(enemyBase)) return;
                var added = EnemiesGetDamage.Add(enemyBase);
                if (added)
                {
                    //foreach (EnemyBase e in this.EnemiesGetDamage)
                    //{
                    enemyBase.onDead = OnEnemyDead;
                    //Debug.LogAssertion($"Added to track {enemyBase.Id}, {enemyBase.GetHashCode()}");
                    //}
                }
            }
        }

        protected override void OnTargetChange(object sender, EventArgs args)
        {
            base.OnTargetChange(sender, args);
            // if (passiveSkill3.Level <= 0) return;
            // ObservableProperty<EnemyBase> enemy = (ObservableProperty<EnemyBase>)sender;
            // if (enemy.Value != null)
            // {
            //     //handle the enemies who is hero's target
            //     enemy.Value.onDead += OnEnemyDead;
            // }
            // foreach (EnemyBase enemyBase in this.EnemiesGetDamage)
            // {
            //     enemyBase.onDead += OnEnemyDead;
            // }
        }

        void OnEnemyDead(Unit victim, Unit killer)
        {
            var enemy = (EnemyBase)victim;
            if (killer == this)
            {
                //emit passive 1, make a explosion on the dead enemy
                int index = RandomFromDistribution.RandomChoiceFollowingDistribution(
                    new List<float>() {
                        1 - PassiveSkill1.SkillData.explosionRatio, PassiveSkill1.SkillData.explosionRatio
                    });
                if (index == 1)
                {
                    if (victim.IsAlive == false)
                        DoSkillPassive1(enemy);
                }
            }

            //var removed =
            EnemiesGetDamage.Remove(enemy);
            // if (removed)
            // {
            //     Debug.LogAssertion($"Removed {enemy.Id}, {enemy.GetHashCode()}");
            // }
        }

        void DoSkillPassive1(EnemyBase enemy)
        {
            var go = ResourceUtils.GetVfxHero("1005_passive_1_impact", enemy.transform.position, Quaternion.identity);
            var scale = PassiveSkill1.SkillData.range;
            go.transform.localScale = new Vector3(scale, scale);
            var damageBox = go.GetComponent<Hero1005Passive1Impact>();

            damageBox.SetOwner(this);
            damageBox.Init();
            damageBox.Trigger(0.1f, 0f);
            //Debug.LogAssertion($"Do 1005 Passive 1 on {enemy.Id}");
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1005_PASSIVE1);
            EazySoundManager.PlaySound(audioClip1);
            //Debug.LogAssertion($"Explode on {enemy.GetHashCode()}");
        }

        public bool IsInPassive2()
        {
            if (PassiveSkill2.Level > 0)
            {
                if (AoeCountTime.TimeOut == false)
                    return true;
                return false;
            }

            return false;
        }

        public void DoSkillPassive3(EnemyBase target)
        {
            if (this.passiveSkill3.Level > 0)
            {
                var stunStatus = new StunStatus(new StatusOverTimeConfig() {
                    creator = this,
                    owner = target,
                    lifeTime = this.passiveSkill3.SkillData.stunInSeconds,
                    statusType = StatusType.Stun
                });

                target.StatusController.AddStatus(stunStatus);
            }
        }

        public override void Revive()
        {
            this._reborn.Execute();
            this._reborn.onComplete = () => {
                this.UnitState.Set(ActionState.None);
                this.bodyHero.enabled = true;
            };
            base.Revive();
            ResearchTarget();
            this.bodyHero.enabled = true;
        }

        public override void Remove()
        {
            Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue = 0;
            base.Remove();
        }

        public override void OnDie()
        {
            this.bodyHero.enabled = false;

            var spine1005 = (Hero1005Spine)this.UnitSpine;
            spine1005.DieLoop();

            if (heroButton)
                heroButton.Deactive();

            if (skillButton)
                skillButton.Deactive();

            RegenHp.Stop();
        }
    }
}