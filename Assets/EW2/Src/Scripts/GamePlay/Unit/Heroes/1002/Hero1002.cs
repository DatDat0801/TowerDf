using System;
using Hellmade.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Hero1002 : HeroBase
    {
        public HeroNormalDamageBox normalAttackBox;
        public Transform pointSpawnBullet;
        public Transform pointSpawnBulletPassive3;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Hero1002Spine(this));

        public override HeroData HeroData
        {
            get
            {
                if (heroData == null)
                {
                    heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1002>();
                }

                return heroData;
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));

        private Vector3 targetActiveSkill;

        //skill
        private Hero1002ActiveSkill activeSkill;
        private Hero1002PassiveSkill1 passiveSkill1;
        private Hero1002PassiveSkill2 passiveSkill2;
        private Hero1002PassiveSkill3 passiveSkill3;

        protected override void Awake()
        {
            base.Awake();

            SetInfo(1002);

            SetRangeAttack();
            attackRange = new AttackRange(this);
            InitSkill();
            SearchTarget.SetBlockNumber(HeroStatBase.blockEnemy);
            attackRange.onComplete = CheckActivePassive;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitCustomStats();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (this == null || !IsAlive || !CanControl) return;

            if (searchTarget.targetAttackType.Value == AttackType.Range)
            {
                if (UnitState.Current == ActionState.AttackRange && passiveSkill3 != null && passiveSkill3.IsReady())
                {
                    Debug.LogWarning("skill3 active");

                    if (SearchTarget.HasTarget)
                    {
                        passiveSkill3.onComplete = () => { CheckActionCallback(); };
                        Flip(searchTarget.target.Value.Transform.position.x);
                        passiveSkill3.Execute();
                    }

                    return;
                }
                else if (UnitState.Current == ActionState.SkillPassive1)
                {
                    if (searchTarget.target.Value == null) return;

                    Flip(searchTarget.target.Value.Transform.position.x);
                    attackRange.Execute(deltaTime);
                    return;
                }
            }

            base.OnUpdate(deltaTime);
        }

        protected override void SetRangeAttack()
        {
            var rangeSearch = SearchTarget as RangerSearchTarget;

            rangeSearch.SetRangeAttack(HeroStatBase.detectRangeAttack);
        }

        #region Skill

        public void InitSkill()
        {
            var heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1002>();

            activeSkill = new Hero1002ActiveSkill(this, heroData.active);
            activeSkill.Init();
            SkillController.SetSkillActive(activeSkill);

            passiveSkill1 = new Hero1002PassiveSkill1(this, heroData.passive1);
            passiveSkill1.Init();
            SkillController.AddSkillPassive(passiveSkill1);

            passiveSkill2 = new Hero1002PassiveSkill2(this, heroData.passive2);
            passiveSkill2.Init();
            SkillController.AddSkillPassive(passiveSkill2);

            passiveSkill3 = new Hero1002PassiveSkill3(this, heroData.passive3);
            passiveSkill3.Init();
            SkillController.AddSkillPassive(passiveSkill3);
        }

        void InitCustomStats()
        {
            //add custom stat
            if (Stats is HeroStats heroStats)
            {
                var skillData = ((Hero1002ActiveSkill)SkillController.SkillActive).SkillData;
                if (skillData != null)
                    heroStats.AddCustomCooldownStat(skillData.cooldown);
            }
        }

        #region Skill Active

        //  Skill active
        public override float GetCoolDownTime()
        {
            var timeCooldown = ((Hero1002ActiveSkill)SkillController.SkillActive).SkillData.cooldown;
            timeCooldown -= timeCooldown * Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
            return timeCooldown;
        }

        public override void ActiveSkillToTarget(Vector3 target, Action callbackCooldown)
        {
            base.ActiveSkillToTarget(target, callbackCooldown);

            if (!CanControl)
                return;

            move.Stop();
            UnitState.Set(ActionState.None);
            targetActiveSkill = target;
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
                Debug.LogWarning("done skill active");
                UnitState.IsLockState = false;
                UnitState.Set(ActionState.None);
                CheckActionCallback();
            };

            Flip(targetActiveSkill.x);

            skillAttack.Execute();

            UnitState.IsLockState = true;

            base.UseSkillActive();
        }
        //

        #endregion

        #region Skill passive

        // Skill passive 1

        private void CheckActivePassive()
        {
            if (!CanControl || passiveSkill1.Level <= 0)
                return;

            if (passiveSkill1 != null)
            {
                if (!passiveSkill1.IsActive())
                    passiveSkill1.CheckActivePassive();

                if (passiveSkill1.IsActive())
                {
                    Debug.LogWarning("skill1 active");
                    UnitState.Set(ActionState.SkillPassive1);
                    attackRange.onComplete = () => { Passive1Complete(); };
                }
            }
        }

        private void Passive1Complete()
        {
            passiveSkill1.Execute();
            attackRange.onComplete = CheckActivePassive;
            attackRange.SetTimeTriggerAttack();
            CheckActionCallback();
        }

        //

        #endregion

        #endregion

        #region Effect

        public void SpawnBullet()
        {
            switch (UnitState.Current)
            {
                case ActionState.SkillPassive1:
                    var rangeSearch = SearchTarget as RangerSearchTarget;
                    if (rangeSearch == null) return;
                    var targets = rangeSearch.SelectTargets(passiveSkill1.dataPassiveCurr.numberArrows);

                    Unit targetAtk = null;

                    if (targets != null && targets.Count > 0)
                    {
                        for (int i = 0; i < passiveSkill1.dataPassiveCurr.numberArrows; i++)
                        {
                            if (i < targets.Count)
                            {
                                targetAtk = targets[i];
                            }
                            else
                            {
                                targetAtk = targets[targets.Count - 1];
                            }

                            if (targetAtk != null)
                            {
                                GameObject bullet = ResourceUtils.GetUnit("bullet_1002_2",
                                    pointSpawnBullet.position, pointSpawnBullet.rotation);
                                if (bullet != null)
                                {
                                    var control = bullet.GetComponent<Bullet1002Passive1>();
                                    control.InitBullet1002Passive1(this, targetAtk, 0.45f);
                                }
                            }
                        }
                    }

                    break;
                case ActionState.AttackRange:
                    if (SearchTarget.HasTarget)
                    {
                        var targetCurr = SearchTarget.target.Value;
                        if (targetCurr != null)
                        {
                            GameObject bullet =
                                ResourceUtils.GetUnit("bullet_1002_0", pointSpawnBullet.position,
                                    pointSpawnBullet.rotation);
                            if (bullet != null)
                            {
                                Bullet1002 control = bullet.GetComponent<Bullet1002>();
                                control.InitBullet1002(this, targetCurr, 0.45f);
                            }
                        }
                        else
                        {
                            Debug.Log("***Sao lai null******");
                        }
                    }

                    break;
                case ActionState.UseSkill:
                    GameObject skillActiveImpact = ResourceUtils.GetVfx("Hero", "1002_skill_active_impact_main",
                        targetActiveSkill, Quaternion.identity);
                    if (skillActiveImpact != null)
                    {
                        var controlImpact = skillActiveImpact.GetComponent<Hero1002SkillImpactMain>();
                        controlImpact.Init(this, activeSkill.SkillData);
                        var audioClip1 = ResourceUtils.LoadSound($"Sounds/Sfx/1002/sfx_hero_miria_active_explode_1");
                        EazySoundManager.PlaySound(audioClip1);
                    }

                    break;
                case ActionState.SkillPassive3:
                    if (SearchTarget.target.Value != null)
                    {
                        var targetPos = SearchTarget.target.Value.transform.position;
                        passiveSkill3.CooldownPassive();
                        GameObject bulletPassive = ResourceUtils.GetUnit("bullet_1002_1",
                            this.pointSpawnBulletPassive3.position,
                            pointSpawnBulletPassive3.rotation);
                        if (bulletPassive != null)
                        {
                            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1002_SHOOT);
                            EazySoundManager.PlaySound(audioClip);

                            Bullet1002SkillActive control = bulletPassive.GetComponent<Bullet1002SkillActive>();
                            control.InitBullet1002Passive(this, targetPos, 0.5f,
                                unit => {
                                    var impact = ResourceUtils.GetVfxHero("1002_skill_3_impact", targetPos,
                                        Quaternion.identity);
                                    if (impact != null)
                                    {
                                        Hero1002PassiveImpact controlImpact =
                                            impact.GetComponent<Hero1002PassiveImpact>();
                                        controlImpact.Init(this, passiveSkill3.DataPassiveCurr);

                                        var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1002_EXPLODE);
                                        EazySoundManager.PlaySound(audioClip1);
                                    }
                                });
                        }
                    }

                    break;
            }
        }

        #endregion

        #region Sfx

        public override void Revive()
        {
            base.Revive();
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_REVIVE);
            EazySoundManager.PlaySound(audioClip);
        }

        #endregion
    }
}