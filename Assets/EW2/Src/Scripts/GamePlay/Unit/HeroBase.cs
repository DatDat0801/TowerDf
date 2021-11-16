using System;
using EW2.Spell;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public abstract class HeroBase : Dummy
    {
        public HeroStatistic Tracking;

        protected HeroData heroData;
        public virtual HeroData HeroData => heroData;

        public HeroStatBase HeroStatBase => HeroData.stats[Level - 1];

        protected HeroButton heroButton;

        protected SkillButton skillButton;

        public SpellUnitBase Spell { get; set; }

        protected AllySearchTarget searchTarget;

        public AllySearchTarget SearchTarget
        {
            get
            {
                if (searchTarget == null)
                {
                    searchTarget = GetComponentInChildren<AllySearchTarget>();
                }

                return searchTarget;
            }
        }

        protected HeroSkillController skillController;


        public virtual HeroSkillController SkillController =>
            skillController ?? (skillController = new HeroSkillController(this));


        /// <summary>
        /// 
        /// </summary>
        protected UnitAction idle;

        /// <summary>
        /// 
        /// </summary>
        protected PolyNavAgentMove move;

        /// <summary>
        /// 
        /// </summary>
        protected AttackMelee attackMelee;

        /// <summary>
        /// 
        /// </summary>
        protected AttackRange attackRange;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction die;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction skillAttack;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction skillPassive1;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction skillPassive2;

        /// <summary>
        /// 
        /// </summary>
        protected UnitAction skillPassive3;

        /// <summary>
        /// 
        /// </summary>
        protected RegenHpOverTime regenHp;

        public RegenHpOverTime RegenHp
        {
            get
            {
                if (regenHp == null)
                    InitRegenHp();
                return regenHp;
            }
        }

        private GameObject aura;

        public GameObject Aura
        {
            get
            {
                if (aura == null)
                {
                    aura = ResourceUtils.GetUnitOther($"{HeroStatBase.id}_aura", transform);

                    aura.transform.position = transform.position;
                }

                return aura;
            }
        }

        private SelectHeroUiController selectCircle;

        public SelectHeroUiController SelectCircle
        {
            get
            {
                if (selectCircle == null)
                {
                    var circle = ResourceUtils.GetUnitOther("select_circle", transform, true);

                    if (circle)
                    {
                        selectCircle = GetComponentInChildren<SelectHeroUiController>();

                        selectCircle.InitSelectCircle(HeroStatBase.id);
                    }
                }

                return selectCircle;
            }
        }


        private GameObject iconRevive;

        protected Vector3 posDefault = Vector3.zero;

        //private float hpMax;

        private HealthPoint _virtualHp;

        protected override void Awake()
        {
            base.Awake();
            UnitType = UnitType.Hero;
        }

        public virtual void OnSetState(ActionState state)
        {
            if (state.Equals(ActionState.Stun))
            {
                move.SetSpeed(0f);
            }
            else
            {
                move.SetSpeed(HeroStatBase.moveSpeed);
            }
        }

        public void SetInfo(int heroId)
        {
            SetId(heroId);

            SetLevel();

            Tracking = new HeroStatistic();

            DamageType = HeroStatBase.damageType;

            if (HealthBar)
            {
                HealthBar.SetHealthBar(Stats.GetStat<HealthPoint>(RPGStatType.Health));
            }

            SearchTarget.SetTargetType(HeroStatBase.searchTarget);

            posDefault = transform.position;
        }

        private void SetLevel()
        {
            var data = UserData.Instance.UserHeroData.GetHeroById(Id);

            if (data != null)
            {
                Level = data.level;
            }
            else
            {
                if (GamePlayController.IsTrialCampaign)
                {
                    var trialHeroData = GameContainer.Instance.Get<MapDataBase>().GetTrialHeroData();
                    if (trialHeroData != null)
                    {
                        var trialData = trialHeroData.GetDataTrial(GamePlayController.CampaignId);
                        Level = trialData.levelHero;
                    }
                }
            }
        }

        protected override void InitAction()
        {
            idle = new Idle(this);

            move = gameObject.AddComponent<PolyNavAgentMove>();

            attackMelee = new AttackMelee(this, HeroStatBase.detectMeleeAttack);

            skillAttack = new SkillAttack(this) {onComplete = CheckActionCallback};

            skillPassive1 = new SkillPassive1(this) {onComplete = CheckActionCallback};

            skillPassive2 = new SkillPassive2(this) {onComplete = CheckActionCallback};

            skillPassive3 = new SkillPassive3(this) {onComplete = CheckActionCallback};

            die = new Die(this);

            UnitState.StartSetState += OnSetState;
        }

        protected void InitRegenHp()
        {
            var healthPoint = Stats.GetStat<HealthPoint>(RPGStatType.Health);

            healthPoint.CurrentValue = healthPoint.StatValue;
            //hpMax = healthPoint.StatValue;

            regenHp = new RegenHpOverTime(new StatusOverTimeConfig() {
                creator = this,
                owner = this,
                lifeTime = float.MaxValue,
                intervalTime = GameConfig.IntervalRegeneration,
                delayTime = HeroStatBase.timeTriggerRegeneration,
                baseValue = Stats.GetStat(RPGStatType.HpRegeneration).StatValue
            }) {Stacks = true};
        }

        public override void OnUpdate(float deltaTime)
        {
            //Debug.LogError($"{this.name}:{UnitState.Current}");
            if (this == null || !IsAlive) return;

            switch (UnitState.Current)
            {
                case ActionState.None:
                    idle.Execute();
                    RegenHp.Execute();
                    break;
                case ActionState.Idle:
                    if (searchTarget.HasTarget && searchTarget.target.Value.IsAlive)
                    {
                        var distance = searchTarget.Distance();
                        if (distance > (this.attackMelee.Range + 0.5f))
                        {
                            if (this.attackMelee.IsAttacking == false)
                                MoveToBlockTarget();
                        }
                        else
                        {
                            var target1 = searchTarget.target.Value;
                            if (target1 && target1.IsAlive)
                            {
                                UnitState.Set(ActionState.AttackMelee);
                            }
                        }
                    }

                    break;
                case ActionState.Move:
                    if (searchTarget.HasTarget)
                    {
                        if (attackMelee.Range >= searchTarget.Distance())
                        {
                            AttackMelee();
                        }
                    }

                    break;
                case ActionState.AttackMelee:
                    if (attackMelee.Range + 0.5f < searchTarget.Distance())
                    {
                        MoveToBlockTarget();
                    }
                    else
                    {
                        var target1 = searchTarget.target.Value;
                        if (target1 && target1.IsAlive)
                        {
                            Flip(target1.Transform.position.x);

                            attackMelee.Execute(deltaTime);

                            RegenHp.Stop();
                        }
                        else
                        {
                            UnitState.Set(ActionState.None);
                        }
                    }

                    break;
                case ActionState.AttackRange:
                    var target = searchTarget.target.Value;
                    if (target && target.IsAlive)
                    {
                        Flip(target.Transform.position.x);

                        attackRange.Execute(deltaTime);

                        RegenHp.Stop();
                    }
                    else
                    {
                        UnitState.Set(ActionState.None);
                    }

                    break;
                case ActionState.Die:
                case ActionState.Stun:
                    break;
                   
            }
        }

        public override void OnEnable()
        {
            UnitState.IsLockState = false;
            UnitState.Set(ActionState.None);

            base.OnEnable();

            searchTarget.target.ValueChanged += OnTargetChange;

            searchTarget.targetAttackType.ValueChanged += OnAttackTypeChange;

            SetActiveSearchTarget(true);

            InitAction();

            SkillController.InitSkill();

            SetRangeBlock();
        }

        public override void OnDisable()
        {
            searchTarget.target.ValueChanged -= OnTargetChange;

            searchTarget.targetAttackType.ValueChanged -= OnAttackTypeChange;

            RegenHp.Stop();

            Destroy(move);

            move = null;

            SetActiveSearchTarget(false);

            SkillController.StopAllSkill();

            UnitState.StartSetState = null;

            base.OnDisable();
        }

        public virtual void MoveToPosDefault()
        {
            if (Vector3.Distance(transform.position, posDefault) < 0.3f) return;

            Move(posDefault, b => {
                UnitState.Set(ActionState.None);

                SetActiveSearchTarget(true);
            });
        }

        protected virtual void OnTargetChange(object sender, EventArgs args)
        {
            if (searchTarget.target.Value != null)
            {
                print("target: " + searchTarget.target.Value.name);
            }
        }

        protected virtual void OnAttackTypeChange(object sender, EventArgs args)
        {
            if (IsAlive == false)
            {
                print($"Hero {name} was died");
                return;
            }

            //Debug.LogWarning("targetType: " + searchTarget.targetAttackType.Value);

            switch (searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    InvokeProxy.Iinvoke.CancelInvoke(this, ResearchTarget);
                    AttackMelee();
                    break;
                case AttackType.Range:
                    InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    InvokeProxy.Iinvoke.CancelInvoke(this, ResearchTarget);
                    AttackRange();
                    break;
                case AttackType.None:
                    if (IsAlive)
                    {
                        InvokeProxy.Iinvoke.Invoke(this, MoveToPosDefault, 2f);
                        InvokeProxy.Iinvoke.Invoke(this, ResearchTarget, 5f);
                    }

                    if (UnitState.Current != ActionState.UseSkill)
                    {
                        this.move.Stop();
                        UnitState.Set(ActionState.None);
                    }

                    break;
            }
        }

        public override void AttackMelee()
        {
            if (CanControl && searchTarget.target.Value != null)
            {
                move.Stop();

                attackMelee.ResetTime();
                Flip(searchTarget.target.Value.Transform.position.x);
                UnitState.Set(ActionState.AttackMelee);
                //Debug.LogAssertion($"attack melee: {searchTarget.target.Value}");
            }
        }

        public override void AttackRange()
        {
            if (CanControl && searchTarget.target.Value != null)
            {
                Flip(searchTarget.target.Value.Transform.position.x);
                UnitState.Set(ActionState.AttackRange);
            }
        }

        public void MoveToBlockTarget()
        {
            if (!CanControl)
                return;

            var posMelee = MathUtils.GetPositionToMeleeAttack(transform.position,
                searchTarget.target.Value.Transform.position, 0.3f);

            Move(posMelee, b => { UnitState.Set(ActionState.None); });
        }

        public virtual void Move(Vector3 target, Action<bool> callback)
        {
            UnitState.Set(ActionState.Move);

            UnitSpine.Move();

            Flip(target.x);

            move.SetDestination(target, callback);

            RegenHp.Stop();
        }

        public override void Remove()
        {
            move.Stop();

            die.onComplete = OnDie;

            die.Execute();

            Stats.ClearStatModifiers();

            StatusController.RemoveAll();

            SetActiveSearchTarget(false);

            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
        }

        public virtual void TouchMove(Vector3 target)
        {
            if (!CanControl)
                return;

            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);

            posDefault = target;

            Tracking.Move();

            SetActiveSearchTarget(false);

            Move(target, b => {
                UnitState.Set(ActionState.None);
                SetActiveSearchTarget(true);
            });
        }

        protected void CheckActionCallback()
        {
            UnitState.Set(ActionState.None);

            if (!SearchTarget.HasTarget)
            {
                return;
            }

            switch (searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    AttackMelee();
                    break;
                case AttackType.Range:
                    AttackRange();
                    break;
            }
        }

        public virtual void ActiveSkillToTarget(Vector3 target, Action callbackCooldown)
        {
            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
        }

        public virtual void CancelActiveSkill()
        {
        }

        public void SelectHero()
        {
            if (CanControl)
            {
                heroButton.SingleSelect();
            }
        }

        public abstract float GetCoolDownTime();

        public virtual float GetReviveTime()
        {
            return Stats.GetStat(RPGStatType.TimeRevive).StatValue;
        }

        public virtual void UseSkillActive()
        {
            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);

            RegenHp.Stop();

            Tracking.UseSkill();
        }

        public virtual void UseSkillPassive1()
        {
            skillPassive1.Execute();
        }

        public virtual void UseSkillPassive2()
        {
            skillPassive2.Execute();
        }

        public virtual void UseSkillPassive3()
        {
            skillPassive3.Execute();
        }

        protected void SetActiveSearchTarget(bool isActive)
        {
            searchTarget.gameObject.SetActive(isActive);
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);

            if (UnitState.Current == ActionState.Idle && !RegenHp.IsExecuting)
                RegenHp.Execute();
        }

        public virtual void Revive()
        {
            ((HeroStats)stats).RefresStats();
            var healthPoint = stats.GetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.CurrentValue = healthPoint.StatValue;

            if (iconRevive)
            {
                LeanPool.Despawn(iconRevive);

                iconRevive = null;
            }

            ResourceUtils.GetVfx("Effects", "fx_common_hero_rebirth", transform.position, Quaternion.identity);

            Tracking.Revive();

            HealthBar.ResetHealthBar();

            EventManager.EmitEventData(GamePlayEvent.onHeroRevive, this);

            gameObject.SetActive(true);
        }

        public virtual void OnDie()
        {
            iconRevive = ResourceUtils.GetUnitOther("icon_revive", null, false);

            if (iconRevive)
            {
                iconRevive.transform.position = transform.position;
            }

            gameObject.SetActive(false);

            if (heroButton)
                heroButton.Deactive();

            if (skillButton)
                skillButton.Deactive();

            RegenHp.Stop();
        }

        public void InitUI(HeroButton heroButton, SkillButton skillButton)
        {
            this.heroButton = heroButton;
            this.skillButton = skillButton;
        }

        public override AttackMelee GetAttackMelee() => attackMelee;

        protected virtual void SetRangeAttack()
        {
        }

        protected virtual void SetRangeBlock()
        {
            SearchTarget.SetRangeBlock(HeroStatBase.detectBlock);
        }

        protected override void CalculateHealthPoint(DamageInfo damageInfo)
        {
            var damage = TakeDamage(damageInfo);
            if (this._virtualHp != null)
            {
                if (this._virtualHp.CurrentValue > 0)
                {
                    this._virtualHp.TakeDamage(damage, damageInfo.creator);
                    Debug.Log(
                        $"{damageInfo} \n=> {name} Damage: {damage.ToString()}, Virtual HP: {this._virtualHp.CurrentValue.ToString()},  Percent: {this._virtualHp.CalculateCurrentPercent().ToString()}");
                    return;
                }
            }

            var health = this.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            health.TakeDamage(damage, damageInfo.creator);

            Debug.Log(
                $"{damageInfo} \n=> {name} Damage: {damage.ToString()}, HP: {health.CurrentValue.ToString()},  Percent: {health.CalculateCurrentPercent().ToString()}");
        }

        public void AddVirtualHealthBar(float hp)
        {
            this._virtualHp = new HealthPoint();
            this._virtualHp.StatBaseValue = hp;
            this._virtualHp.CurrentValue = hp;
            var virtualHpBar = StatusBar.AddVirtualHpBar().GetComponent<VirtualHpBarController>();
            virtualHpBar.SetHealthBar(this._virtualHp);
        }

        public override void Flip(float positionX)
        {
            var flip = Transform.position.x > positionX;
            if ((Transform.localScale.x > 0 && flip) || (Transform.localScale.x < 0 && flip == false))
            {
                var localScale = Transform.localScale;
                localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
                Transform.localScale = localScale;
            }

            FlipComponent();
        }

        public void ResearchTarget()
        {
            SetActiveSearchTarget(false);
            SetActiveSearchTarget(true);
        }
        public void DecreaseCooldown(float amount)
        {
            this.skillButton.DecreaseCooldown(amount);
        }
#if UNITY_EDITOR
        [SerializeField] private ActionState debugState;
        private void FixedUpdate()
        {
            this.debugState = UnitState.Current;
        }
#endif
    }
}