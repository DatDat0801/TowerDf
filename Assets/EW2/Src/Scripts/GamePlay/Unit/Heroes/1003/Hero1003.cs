using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EW2.Spell;
using Hellmade.Sound;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class Hero1003 : HeroBase
    {
        public HeroNormalDamageBox normalAttackBox;
        public Hero1003PassiveSkill2Collider counterAttackBox;
        public HeroAttractiveCollider passive1Collider;

        public enum Form
        {
            Form1,
            Form2
        }

        public Form FormState { get; set; }
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Hero1003Spine(this));

        public override HeroData HeroData
        {
            get
            {
                if (heroData == null)
                {
                    heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1003>();
                }

                return heroData;
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));

        //skill
        private Hero1003ActiveSkill activeSkill;
        public Hero1003PassiveSkill1 passiveSkill1;
        public Hero1003PassiveSkill2 passiveSkill2;
        private Hero1003PassiveSkill3 passiveSkill3;

        /// <summary>
        /// 1~passive1, 2~passive2, 0~None
        /// </summary>
        private int activatedPassiveSkill;

        private VfxOverTime healVfx;

        protected override void Awake()
        {
            base.Awake();
            //Debug.Log(UserData.Instance.UserHeroData);
            SetInfo(1003);
            InitSkill();
            SearchTarget.SetBlockNumber(HeroStatBase.blockEnemy);
            healVfx = new VfxOverTime() {owner = this, vfxName = "fx_status_bufhealth"};
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitCustomStats();
            EventManager.StartListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.StopListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        }

        private void Initialize()
        {
            //connect this hero with its equipped spell
            if (Spell != null)
                Spell.OnSkillPassive += OnPassiveSkill;
        }

        private void OnPassiveSkill(Unit creator, Unit target)
        {
            //if (target.IsAlive) return;
            if (passiveSkill3.Level <= 0) return;

            try
            {
                var spellUnitBase = (SpellUnitBase)creator;
                var enemy = (EnemyBase)target;
                if (target == null) return;
                if (spellUnitBase.Hero.Equals(this))
                {
                    enemy.onDead = OnEnemyDead;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        protected override void OnTargetChange(object sender, EventArgs args)
        {
            //base.OnTargetChange(sender, args);
            if (passiveSkill3.Level <= 0) return;
            ObservableProperty<EnemyBase> enemy = (ObservableProperty<EnemyBase>)sender;
            if (enemy.Value != null)
            {
                //enemy.Value.onDead = null;
                enemy.Value.onDead += OnEnemyDead;
            }
        }

        void OnEnemyDead(Unit victim, Unit killer)
        {
            if (killer.Equals(this))
            {
                //Action skill 3
                //Debug.LogAssertion(
                //    $"<color=orange>Health of {Id} before add: {Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}</color>");
                var health = Stats.GetStat<HealthPoint>(RPGStatType.Health);
                var addedHealth = passiveSkill3.CurrentPassiveData.recoverHpPerKilledEnemy;
                health.CurrentValue += addedHealth;
                //vfx
                // StatusController.AddStatus(
                //     new HealOverTime(new StatusOverTimeConfig() {lifeTime = 1f, owner = this}));
                healVfx.DoPlayEffect(0.8f, 0);
                //sfx
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_PASSIVE_3);
                EazySoundManager.PlaySound(audioClip);
                //Debug.Log(
                //   $"<color=orange>Enemy {victim.Id} is killed by {killer.Id}, health added {addedHealth} health now {Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}</color>");
            }

            if (killer.GetType().IsSubclassOf(typeof(SpellUnitBase)))
            {
                var spellUnitBase = (SpellUnitBase)killer;
                if (spellUnitBase.Hero.Equals(this))
                {
                    //Action skill 3
                    //Debug.LogAssertion(
                    //    $"<color=green>Health {Id} before add by Spell: {Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}</color>");
                    var health = Stats.GetStat<HealthPoint>(RPGStatType.Health);
                    var addedHealth = passiveSkill3.CurrentPassiveData.recoverHpPerKilledEnemy;
                    health.CurrentValue += addedHealth;

                    healVfx.DoPlayEffect(1f, 0);
                    //sfx
                    var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_PASSIVE_3);
                    EazySoundManager.PlaySound(audioClip);
                    //Debug.Log(
                    //    $"<color=green>Enemy {victim.Id} is killed by {Spell.Id} on hero {spellUnitBase.Hero.Id}, health added {addedHealth} health now {Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}</color>");
                }
            }
        }

        public override void UseSkillActive()
        {
            UnitState.Set(ActionState.UseSkill);

            //Debug.LogAssertion($"current state: {UnitState.Current}");
            base.UseSkillActive();

            FormState = Form.Form2;
            move.Stop();
            skillAttack.Execute();
            SkillController.RunActiveSkill();
            CheckAction();
            OverrideSkillButton();
        }

        async void OverrideSkillButton()
        {
            //Add override skill button in a while
            var prefab = ResourceUtils.Get<GameObject>($"Art/Prefabs/GamePlays/UI/override_1003_button");
            var go = LeanPool.Spawn(prefab, skillButton.transform);

            var button = go.GetComponent<SkillButtonOverride>();
            button.CooldownTime = ((Hero1003ActiveSkill)SkillController.SkillActive).SkillData.duration;
            button.ResetCooldown();

            await UniTask.Delay(TimeSpan.FromSeconds(button.CooldownTime));
            LeanPool.Despawn(go);
        }


        public override void OnUpdate(float deltaTime)
        {
            // if (!IsAlive || !CanControl) return;
            // if (move.currentSpeed > 0)
            // {
            //     UnitState.Set(ActionState.Move);
            // }
            if(UnitState.Current == ActionState.SkillPassive2) return;
            base.OnUpdate(deltaTime);


            if (!activeSkill.IsInDuration() && FormState == Form.Form2)
            {
                move.Stop();
                FormState = Form.Form1;
                var spine = (Hero1003Spine)UnitSpine;
                spine.TurnIntoForm1(CheckActionCallback);

                //Debug.LogAssertion($"Change to state:" + FormState);
            }
        }

        public override float GetCoolDownTime()
        {
            var timeCooldown = ((Hero1003ActiveSkill)SkillController.SkillActive).SkillData.cooldown;
            timeCooldown -= timeCooldown * Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
            return timeCooldown;
        }

        public void InitSkill()
        {
            var heroData = (HeroData1003)HeroData;

            activeSkill = new Hero1003ActiveSkill(this, heroData.active);
            activeSkill.Init();
            SkillController.SetSkillActive(activeSkill);

            passiveSkill1 = new Hero1003PassiveSkill1(this, heroData.passive1);
            passiveSkill1.Init();
            SkillController.AddSkillPassive(passiveSkill1);

            passiveSkill2 = new Hero1003PassiveSkill2(this, heroData.passive2);
            passiveSkill2.Init();
            SkillController.AddSkillPassive(passiveSkill2);

            passiveSkill3 = new Hero1003PassiveSkill3(this, heroData.passive3);
            passiveSkill3.Init();
            SkillController.AddSkillPassive(passiveSkill3);

            //
            FormState = Form.Form1;
        }

        void InitCustomStats()
        {
            //add custom stat
            if (Stats is HeroStats heroStats)
            {
                var skillData = ((Hero1003ActiveSkill)SkillController.SkillActive).SkillData;
                if (skillData != null)
                    heroStats.AddCustomCooldownStat(skillData.cooldown);
            }
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);
            if (!IsAlive) return;

            if (damageInfo.showVfxNormalAtk)
            {
                int i1 = 0, i2 = 0;
                if (passiveSkill1.Level > 0 && passiveSkill1.CanCastSkill())
                {
                    var passive1Data = passiveSkill1.CurrentPassiveData;
                    i1 = RandomFromDistribution.RandomChoiceFollowingDistribution(
                        new List<float>() {1 - passive1Data.roarRatio, passive1Data.roarRatio});
                }

                if (passiveSkill2.Level > 0)
                {
                    var passive2Data = passiveSkill2.CurrentPassiveData;
                    i2 = RandomFromDistribution.RandomChoiceFollowingDistribution(new List<float>() {
                        1 - passive2Data.counterAttackRatio, passive2Data.counterAttackRatio
                    });
                }

                //Calculate the possibility of the Passive1 and Passive2
                if ((i1 == 1 && i2 == 1) || (i1 == 1 && i2 == 0))
                {
                    activatedPassiveSkill = 1;
                }
                else if (i2 == 1)
                {
                    activatedPassiveSkill = 2;
                }
                else
                {
                    activatedPassiveSkill = 0;
                }

                if (activatedPassiveSkill == 0) return;
                //if(move.currentSpeed > 0) return;
                UseSkillPassive1();
                UseSkillPassive2();
            }
        }

        public override void UseSkillPassive1()
        {
            if (!CanControl) return;

            if (activatedPassiveSkill == 1 && passiveSkill1.CanCastSkill() && !activeSkill.IsInAnimation() &&
                UnitState.Current != ActionState.Move)
            {
                //hero action
                skillPassive1.Execute();
                //skill
                passiveSkill1.Execute();
                //Debug.LogAssertion("EXE PASSIVE 1");
                move.Stop();
                CheckAction();
            }
        }

        public override void UseSkillPassive2()
        {
            //base.UseSkillPassive2();
            if (!CanControl) return;
            if (activatedPassiveSkill == 2 && passiveSkill2.CanCastSkill() && SearchTarget.HasTarget &&
                UnitState.Current != ActionState.Move)
            {
                UnitState.Set(ActionState.SkillPassive2);
                //action
                skillPassive2.Execute();
                passiveSkill2.Execute();
                Debug.LogAssertion("EXE PASSIVE 2");
                move.Stop();

                //UnitState.Set(ActionState.Idle);
                //skillPassive2.Execute();
                this.skillPassive2.onComplete += () => {UnitState.Set(ActionState.None); };
                //CheckAction();
            }
        }


        public void ExecutePassiveSkill1()
        {
            //Debug.LogAssertion("Triggered Collider for Passive Skill 1");
            passive1Collider.Trigger(0f, 0.2f);
        }

        //reset time for random animation
        public void ResetTimeTriggerAttackMelee(string animationName)
        {
            attackMelee.UpdateTimeTrigger(animationName);
        }

        public override void Revive()
        {
            base.Revive();
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_REVIVE);
            EazySoundManager.PlaySound(audioClip);
        }

        public override void OnDie()
        {
            FormState = Form.Form1;

            base.OnDie();
        }

        private void CheckAction()
        {
            if (!SearchTarget.HasTarget)
            {
                return;
            }

            switch (searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    AttackMelee();
                    break;
            }
        }
    }
}