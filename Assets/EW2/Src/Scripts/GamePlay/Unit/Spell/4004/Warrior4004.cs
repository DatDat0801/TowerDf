using System;
using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2.Spell
{
    public class Warrior4004 : Dummy
    {
        public WarriorDamageBox attackMelee1Box;
        public WarriorDamageBox attackMelee2Box;

        private Spell4004Data _spell4004Data;

        public Spell4004Data Spell4004Data
        {
            get
            {
                if (this._spell4004Data == null)
                {
                    this._spell4004Data = GameContainer.Instance.Get<UnitDataBase>().Get<Spell4004Data>();
                }

                return this._spell4004Data;
            }
        }

        public Warrior4004Stat WarriorStatBase => Spell4004Data.warriorStats[Level - 1];
        public SpellStatBase SpellStatBase => Spell4004Data.spellStats[Level - 1];

        //private SpellStatBase _spellStatBase;

        private AllySearchTarget _searchTarget;

        public AllySearchTarget SearchTarget
        {
            get
            {
                if (this._searchTarget == null)
                {
                    this._searchTarget = GetComponentInChildren<AllySearchTarget>();
                }

                return this._searchTarget;
            }
        }

        protected UnitAction idle;
        private Appear _appear;
        protected PolyNavAgentMove move;
        protected AttackMelee attackMelee;
        protected AttackRange attackRange;

        protected UnitAction die;

        // protected MultiMove multiMove;
        // protected SingleMove singleMove;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Warrior4004Spine(this));

        public override RPGStatCollection Stats =>
            stats ?? (stats = new Warrior4004Stats(this, WarriorStatBase));

        private Spell4004 _spell4004;

        protected override void Awake()
        {
            base.Awake();
            this._spell4004 = FindObjectOfType<Spell4004>();
            Init(this._spell4004.Level);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitAction();
            _searchTarget.targetAttackType.ValueChanged += OnAttackTypeChange;
            this._searchTarget.target.ValueChanged += OnTargetChange;
            SetActiveSearchTarget(true);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _searchTarget.targetAttackType.ValueChanged -= OnAttackTypeChange;
            this._searchTarget.target.ValueChanged -= OnTargetChange;
            Destroy(move);
        }

        protected virtual void OnTargetChange(object sender, EventArgs args)
        {
            if (_searchTarget.target.Value != null)
            {
                print("target: " + _searchTarget.target.Value.name);
                var distance = _searchTarget.Distance();
                if (distance > 1)
                {
                    MoveToBlockTarget();
                }
            }
        }

        public void Init(int level)
        {
            this.Level = level;
            UnitType = UnitType.Hero;

            DamageType = this.WarriorStatBase.damageType;

            if (HealthBar)
            {
                HealthBar.SetHealthBar(Stats.GetStat<HealthPoint>(RPGStatType.Health));
            }

            var healthPoint = this.stats.GetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.CurrentValue = healthPoint.StatValue;

            SearchTarget.SetTargetType(this.WarriorStatBase.searchTarget);
            SearchTarget.SetBlockNumber(this.WarriorStatBase.blockEnemy);
        }


        public override void OnUpdate(float deltaTime)
        {
            if (this == null || !IsAlive) return;

            switch (UnitState.Current)
            {
                case ActionState.None:
                    this.move.Stop();
                    idle.Execute();
                    break;
                case ActionState.Idle:
                    // if (this._searchTarget.HasTarget == false)
                    // {
                    //     break;
                    // }
                    var distance = _searchTarget.Distance();
                    if (distance > this.attackMelee.Range)
                    {
                        if (this.attackMelee.IsAttacking == false)
                            MoveToBlockTarget();
                    }
                    else
                    {
                        var target1 = _searchTarget.target.Value;
                        if (target1 && target1.IsAlive)
                        {
                            //Flip(target1.Transform.position.x);
                            UnitState.Set(ActionState.AttackMelee);
                            //attackMelee.Execute(deltaTime);
                        }
                    }

                    break;
                case ActionState.Move:
                    if (_searchTarget.HasTarget)
                    {
                        if (this.attackMelee.Range >= _searchTarget.Distance())
                        {
                            AttackMelee();
                        }
                    }

                    break;
                case ActionState.AttackMelee:
                    if (attackMelee.Range < _searchTarget.Distance())
                    {
                        if (this.attackMelee.IsAttacking == false)
                            MoveToBlockTarget();
                    }
                    else if (attackMelee.Range >= _searchTarget.Distance())
                    {
                        var target1 = _searchTarget.target.Value;
                        if (target1 && target1.IsAlive)
                        {
                            Flip(target1.Transform.position.x);

                            attackMelee.Execute(deltaTime);
                        }
                        else
                        {
                            UnitState.Set(ActionState.None);
                        }
                    }

                    break;
                case ActionState.AttackRange:
                    var target = _searchTarget.target.Value;
                    if (target && target.IsAlive)
                    {
                        Flip(target.Transform.position.x);

                        attackRange.Execute(deltaTime);
                    }
                    else
                    {
                        UnitState.Set(ActionState.None);
                    }

                    break;
                case ActionState.Die:
                case ActionState.Stun:
                    break;
                case ActionState.Appear:
                    this._appear.Execute();
                    this._appear.onComplete = () => UnitState.Set(ActionState.None);
                    break;
            }
        }

        public override void Remove()
        {
            move.Stop();

            die.onComplete = OnDie;

            die.Execute();

            Stats.ClearStatModifiers();

            StatusController.RemoveAll();

            SetActiveSearchTarget(false);

            //InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
        }

        protected override void InitAction()
        {
            move = this.gameObject.AddComponent<PolyNavAgentMove>();
            idle = new Idle(this);

            attackMelee = new AttackMelee(this, this.WarriorStatBase.detectMeleeAttack);

            die = new Die(this);
            this._appear = new Appear(this);

            var healthPoint = this.stats.GetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.CurrentValue = healthPoint.StatValue;

            UnitState.StartSetState = OnSetState;
            UnitState.IsLockState = false;
            UnitState.Set(ActionState.Appear);
            InvokeProxy.Iinvoke.Invoke(this, InitNone, 0.5f);
        }

        void InitNone()
        {
            UnitState.Set(ActionState.None);
        }

        public override void AttackMelee()
        {
            if (CanControl && _searchTarget.target.Value != null)
            {
                move.Stop();

                attackMelee.ResetTime();
                Flip(_searchTarget.target.Value.Transform.position.x);
                UnitState.Set(ActionState.AttackMelee);
            }
        }

        public override void AttackRange()
        {
            if (CanControl && _searchTarget.target.Value != null)
            {
                Flip(_searchTarget.target.Value.Transform.position.x);
                UnitState.Set(ActionState.AttackRange);
            }
        }

        public void ResetTimeTriggerAttackMelee(string animationName)
        {
            attackMelee.UpdateTimeTrigger(animationName);
        }

        private void OnDie()
        {
            LeanPool.Despawn(this.gameObject);
        }

        protected void SetActiveSearchTarget(bool isActive)
        {
            _searchTarget.gameObject.SetActive(isActive);
        }

        public void OnSetState(ActionState state)
        {
            if (state == ActionState.Stun)
            {
                move.SetSpeed(0f);
            }
            else
            {
                move.SetSpeed(this.WarriorStatBase.moveSpeed);
            }
        }

        public void MoveToBlockTarget()
        {
            if (!CanControl)
                return;

            var posMelee = MathUtils.GetPositionToMeleeAttack(transform.position,
                _searchTarget.target.Value.Transform.position, 0.3f);

            Move(posMelee, b => { UnitState.Set(ActionState.None); });
        }

        public void Move(Vector3 target, Action<bool> callback)
        {
            UnitState.Set(ActionState.Move);

            UnitSpine.Move();

            Flip(target.x);

            move.SetDestination(target, callback);
        }

        protected virtual void OnAttackTypeChange(object sender, EventArgs args)
        {
            if (IsAlive == false)
            {
                print($"Hero {name} was died");
                return;
            }

            Debug.LogWarning("targetType: " + _searchTarget.targetAttackType.Value);

            switch (_searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    //InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    AttackMelee();
                    break;
                case AttackType.Range:
                    //InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    AttackRange();
                    break;
                case AttackType.None:
                    if (UnitState.Current != ActionState.UseSkill)
                    {
                        this.move.Stop();
                        UnitState.Set(ActionState.None);
                    }

                    break;
            }
        }

        public ActionState debugState;

        private void Update()
        {
            this.debugState = UnitState.Current;
        }

        public override void Flip(float positionX)
        {
            var flip = Transform.position.x > positionX;
        
            if ((UnitSpine.Skeleton.ScaleX > 0 && flip) || (UnitSpine.Skeleton.ScaleX < 0 && flip == false))
            {
                var positionMelee1 = attackMelee1Box.transform.parent.transform.localPosition;
                var positionMelee2 = attackMelee2Box.transform.parent.transform.localPosition;
        
                positionMelee1 = new Vector3(-positionMelee1.x, positionMelee1.y, positionMelee1.z);
                positionMelee2 = new Vector3(-positionMelee2.x, positionMelee2.y, positionMelee2.z);
        
                attackMelee1Box.transform.parent.transform.localPosition = positionMelee1;
                attackMelee2Box.transform.parent.transform.localPosition = positionMelee2;
                
                //scale for melee1 fx
                var localScale = attackMelee1Box.transform.parent.transform.localScale;
                localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
                attackMelee1Box.transform.parent.transform.localScale = localScale;
            }
        
            base.Flip(positionX);
        }
    }
}