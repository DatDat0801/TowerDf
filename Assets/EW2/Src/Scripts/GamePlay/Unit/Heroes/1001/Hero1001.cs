using System;
using System.CodeDom;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using UnityEngine;

namespace EW2
{
    public class Hero1001 : HeroBase
    {
        public HeroNormalDamageBox normalAttackBox;

        public Hero1001ActiveSkillCollider activeSkill;

        public Hero1001PassiveSkill1Collider passiveSkill1;

        public Hero1001PassiveSkill3Collider passiveSkill3;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Hero1001Spine(this));

        public override HeroData HeroData
        {
            get
            {
                if (heroData == null)
                {
                    heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1001>();
                }

                return heroData;
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));

        protected override void Awake()
        {
            base.Awake();

            SetInfo(1001);

            InitSkill();

            SearchTarget.SetBlockNumber(HeroStatBase.blockEnemy);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            SkillController.RunPassiveSkill();

            InitCustomStats();
        }

        public override float GetCoolDownTime()
        {
            var timeCooldown = ((Hero1001ActiveSkill)SkillController.SkillActive).SkillData.cooldown;
            var reduction = Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
            timeCooldown -= timeCooldown * reduction;
            return timeCooldown;
        }

        public override void UseSkillActive()
        {
            if (CanControl == false)
                return;

            SkillController.RunActiveSkill();

            skillAttack.onComplete = () => {
                UnitState.IsLockState = false;
                UnitState.Set(ActionState.None);
                CheckActionCallback();
            };

            skillAttack.Execute();

            UnitState.IsLockState = true;

            base.UseSkillActive();
        }

        private Vector3 _target;
        public override void ActiveSkillToTarget(Vector3 target, Action callbackCooldown)
        {
            posDefault = target;

            base.ActiveSkillToTarget(target, callbackCooldown);

            if (!CanControl)
                return;

            move.Stop();

            

            // Move(target, b => {
            //     if (b)
            //     {
            //         UseSkillActive();
            //         callbackCooldown?.Invoke();
            //         SetActiveSearchTarget(true);
            //     }
            // });
            Teleport(target, callbackCooldown);
        }
        private async void Teleport(Vector3 target, Action callbackCooldown)
        {
            var valid = this.move.IsPointValidOnMap(target);
            if (valid)
            {
                this._target = target;
                UseSkillActive();
                callbackCooldown?.Invoke();
                
            }
        }

        public void StartTeleport()
        {
            SetActiveSearchTarget(false);
            this.healthBar.gameObject.SetActive(false);
        }

        public void OnTeleport()
        {
            transform.position = this._target;
        }

        public void EndTeleport()
        {
            SetActiveSearchTarget(true);
            this.healthBar.gameObject.SetActive(true);
        }
        
        public override void CancelActiveSkill()
        {
            if (UnitState.Current == ActionState.Move)
            {
                move.Stop();
                UnitState.Set(ActionState.None);
                SetActiveSearchTarget(true);
            }
        }

        public void InitSkill()
        {
            var heroData = (HeroData1001)HeroData;

            SkillController.SetSkillActive(new Hero1001ActiveSkill(this, heroData.active));

            SkillController.AddSkillPassive(new Hero1001PassiveSkill1(this, heroData.passive1));

            SkillController.AddSkillPassive(new Hero1001PassiveSkill2(this, heroData.passive2));

            SkillController.AddSkillPassive(new Hero1001PassiveSkill3(this, heroData.passive3));
        }

        private void InitCustomStats()
        {
            //add custom stat
            if (Stats is HeroStats heroStats)
            {
                var skillData = ((Hero1001ActiveSkill)SkillController.SkillActive).SkillData;
                if (skillData != null)
                {
                    // var reduction = Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
                    // var cd = skillData.cooldown - skillData.cooldown * reduction;
                    heroStats.AddCustomCooldownStat(skillData.cooldown);
                }
            }
        }

        public void SetAttackMelee(AttackMelee attackMelee)
        {
            this.attackMelee = attackMelee;
        }


        public override void Revive()
        {
            base.Revive();
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_REVIVE);
            EazySoundManager.PlaySound(audioClip);
        }
    }
}