using System;
using Invoke;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Soldier2004 : SoldierBase
    {
        public SoldierNormalDamageBox normalAttackBox;
        public Soldier2004PassiveSkillCollider passiveAttackBox;
        public Tower2004 Tower { get; protected set; }

        protected SoldierRallyController rallyController;
        public SoldierRallyController RallyController => rallyController;

        protected PolyNavAgentMove move;
        protected AttackMelee attackMelee;
        protected SkillAttack skillAttack;
        protected Die die;
        protected RegenHpOverTime regenHp;

        private bool unlockSkillCounter, activeSkillCounter;
        public TowerData2004.Soldier2004Data SoldierData { get; private set; }

        private TowerData2004.Skill1 dataSkillCounter;
        public TowerData2004.Skill1 DataSkillCounter => dataSkillCounter;

        private float timeDelayToFlip;
        public bool IsGhost { get; private set; }
        private const int MAX_SKILL_LEVEL = 6;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Soldier2004Spine(this));

        public override RPGStatCollection Stats
        {
            get
            {
                if (Tower.Stats != null && stats == null)
                {
                    stats = new Soldier2004Stats(this, SoldierData);
                }

                return stats;
            }
        }

        public override AttackMelee GetAttackMelee()
        {
            return attackMelee;
        }

        private GameObject aura;

        public override GameObject Aura
        {
            get
            {
                if (aura == null)
                {
                    aura = ResourceUtils.GetUnitOther($"soldier_aura", transform);

                    aura.transform.position = transform.position;
                }

                return aura;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if (this == null) return false;
                if (gameObject == null) return false;
                if (UnitState.Current == ActionState.Invisible) return true;
                return gameObject.activeSelf && Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue > 0;
            }
        }

        public override void OnEnable()
        {
            HealthBar.gameObject.SetActive(true);
            SearchTarget.target.ValueChanged += HandleAttackTarget;
            SearchTarget.targetAttackType.ValueChanged += HandleAttackType;
            IsGhost = false;
            base.OnEnable();
        }

        private void OnDestroy()
        {
            if (InvokeProxy.Iinvoke != null)
            {
                InvokeProxy.Iinvoke.CancelInvoke(this);
            }
        }

        public override void OnDisable()
        {
            regenHp.Stop();

            SearchTarget.target.ValueChanged -= HandleAttackTarget;
            SearchTarget.targetAttackType.ValueChanged -= HandleAttackType;

            if (InvokeProxy.Iinvoke != null)
            {
                InvokeProxy.Iinvoke.CancelInvoke(this);
            }

            searchTarget.target.Value = null;
            base.OnDisable();

        }

        public void InitDataSoldier(int id, Tower2004 owner, Vector3 pointRally,
            TowerData2004.Soldier2004Data dataSoldier)
        {
            this.owner = this.Tower = owner;

            this.idSoldier = id;

            this.SoldierData = dataSoldier;

            DamageType = owner.DamageType;

            SetId(owner.Id);

            SetSkinSoldier(this.SoldierData.level);

            InitAction();

            SearchTarget.SetBlockNumber(this.SoldierData.blockEnemy);

            if (rallyController == null)
            {
                rallyController = new SoldierRallyController(id);
            }

            rallyController.Rally(pointRally);

            Rally();

            HealthBar.gameObject.SetActive(true);
            if (HealthBar)
            {
                HealthBar.SetHealthBar(Stats.GetStat<HealthPoint>(RPGStatType.Health));
            }
        }

        protected override void InitAction()
        {
            base.InitAction();

            if (move == null)
                move = gameObject.AddComponent<PolyNavAgentMove>();
            attackMelee = new AttackMelee(this, 0.3f);
            die = new Die(this);
            skillAttack = new SkillAttack(this);
            regenHp = new RegenHpOverTime(new StatusOverTimeConfig()
            {
                creator = this,
                owner = this,
                lifeTime = float.MaxValue,
                intervalTime = GameConfig.IntervalRegeneration,
                delayTime = SoldierData.timeTriggerRegeneration,
                baseValue = SoldierData.hpRegeneration
            })
            {
                Stacks = true
            };
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsAlive) return;

            switch (UnitState.Current)
            {
                case ActionState.Idle:
                    if (searchTarget.target.Value != null)
                    {
                        AttackMelee();
                    }

                    break;
                case ActionState.None:
                    idle.Execute();
                    regenHp.Execute();

                    if (searchTarget.target.Value != null)
                    {
                        AttackMelee();
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
                    if (!UnitState.CanControl() || searchTarget.target.Value == null)
                    {
                        UnitState.Set(ActionState.None);
                        attackMelee.ResetTime();
                        break;
                    }

                    if (attackMelee.Range + 0.2f < searchTarget.Distance())
                    {
                        MoveToTarget();
                    }
                    else
                    {
                        if (searchTarget.target.Value != null)
                            Flip(searchTarget.target.Value.Transform.position.x);
                        attackMelee.Execute(deltaTime);
                        regenHp.Stop();
                    }

                    break;
            }
        }

        #region Action

        private void HandleAttackTarget(object sender, EventArgs e)
        {
            if (searchTarget.target.Value != null)
            {
                //Debug.Log("target: " + searchTarget.target.Value.name);
            }
        }

        private void HandleAttackType(object sender, EventArgs e)
        {
            //Debug.Log("targetType: " + searchTarget.targetAttackType.Value);

            switch (searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    AttackMelee();
                    break;
                case AttackType.None:
                    if (IsAlive)
                        InvokeProxy.Iinvoke.Invoke(this, MoveToPosDefault, 2f);
                    break;
            }
        }

        private void MoveToPosDefault()
        {
            Rally();
        }

        public void MoveToTarget()
        {
            if (!CanControl)
                return;

            Move(searchTarget.target.Value.Transform.position, b => { UnitState.Set(ActionState.None); });
        }

        public void Move(Vector3 target, Action<bool> callback, bool isRally = false)
        {
            if (isRally)
                UnitState.Set(ActionState.Rally);
            else
                UnitState.Set(ActionState.Move);

            UnitSpine.Move();

            Flip(target.x);

            regenHp.Stop();

            if (move == null)
                move = GetComponent<PolyNavAgentMove>();

            move.SetDestination(target, callback);
        }

        public override void AttackMelee()
        {
            if (!UnitState.CanControl() || searchTarget.target.Value == null || !IsAlive)
                return;

            move.Stop();
            attackMelee.ResetTime();

            Flip(searchTarget.target.Value.Transform.position.x);
            if (unlockSkillCounter)
                attackMelee.onComplete = CheckCounterAttack;
            UnitState.Set(ActionState.AttackMelee);
        }

        public void UseSkillCounter()
        {
            if (!UnitState.CanControl() || searchTarget.target.Value == null || !unlockSkillCounter)
                return;

            move.Stop();
            attackMelee.ResetTime();
            Flip(searchTarget.target.Value.Transform.position.x);
            skillAttack.onComplete = () => { UnitState.Set(ActionState.AttackMelee); };
            skillAttack.Execute();
            regenHp.Stop();
        }

        public void Rally()
        {
            if (!UnitState.CanControl())
                return;

            if (rallyController != null)
            {
                SetActiveSearchTarget(false);
                //
                Move(rallyController.RallyPoint, b =>
                {
                    UnitState.Set(ActionState.None);
                    SetActiveSearchTarget(true);
                }, true);
            }
        }

        private void SetActiveSearchTarget(bool isActive)
        {
            searchTarget.gameObject.SetActive(isActive);
        }

        private void Reborn()
        {
            UnitState.IsLockState = false;
            UnitState.Set(ActionState.Rally);
            IsGhost = false;
            StatusController.RemoveAll();

            if (gameObject == null)
                return;

            if (move == null)
                move = GetComponent<PolyNavAgentMove>();

            SetSkinSoldier(this.SoldierData.level);
            activeSkillCounter = false;

            searchTarget.target.Value = null;

            Transform.position = Tower.Transform.position;

            Transform.SetParent(owner.Transform);

            if (HealthBar)
            {
                HealthBar.SetHealthBar(Stats.GetStat<HealthPoint>(RPGStatType.Health));
            }

            gameObject.SetActive(true);

            Rally();

            //Debug.Log($"Reborn, State: {UnitState.Current}, Id: {IdSoldier}, IsGhost: {IsGhost}, Rally from Reborn");
        }

        public void RebornImmediate()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this, Reborn);

            Reborn();
        }


        public override void Remove()
        {
            if (Tower.BonusStat.level >= MAX_SKILL_LEVEL)
            {
                IsGhost = true;
                SetSkinSoldier(this.SoldierData.level);
                Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue = 1;
                if (InvokeProxy.Iinvoke != null)
                {
                    InvokeProxy.Iinvoke.Invoke(this, DoDie, Tower.BonusStat.ghostExistTime);
                }
            }
            else
            {
                DoDie();
            }
        }

        private void DoDie()
        {
            IsGhost = false;
            StatusController.RemoveAll();

            SetActiveSearchTarget(false);
            activeSkillCounter = false;
            die.onComplete = () =>
            {
                var timeRevive = Stats.GetStat<TimeRevive>(RPGStatType.TimeRevive).StatValue;
                Debug.Log($"[Soldier {gameObject.name}] Reborn after {timeRevive}s");
                gameObject.SetActive(false);
                Stats.ConfigureStats();
                InvokeProxy.Iinvoke.Invoke(this, Reborn, timeRevive);
            };
            die.Execute();
        }
        
        public override void GetHurt(DamageInfo damageInfo)
        {
            if (IsGhost)
            {
                return;
            }

            if (unlockSkillCounter)
            {
                if (damageInfo.attackType == AttackType.Melee)
                    activeSkillCounter = Random.Range(0f, 1f) < dataSkillCounter.chance;
                if (activeSkillCounter)
                    Debug.Log($"[Soldier {gameObject.name}] Active passive 1");
            }

            base.GetHurt(damageInfo);
            regenHp.Stop();
        }

        public void CheckCounterAttack()
        {
            if (!UnitState.CanControl())
                return;

            if (activeSkillCounter)
            {
                activeSkillCounter = false;
                UseSkillCounter();
            }
        }

        #endregion

        public void OnRaise(int levelTower, TowerData2004.Soldier2004Data newData)
        {
            this.SoldierData = newData;
            ((Soldier2004Stats) Stats).UpdateStats(SoldierData);
            SetSkinSoldier(levelTower);
            if (!this.IsAlive)
                RebornImmediate();

            ShowEffectUpgrade();
        }

        public void OnRaiseSkillCounter(TowerData2004.Skill1 dataSkill)
        {
            if (!unlockSkillCounter)
                unlockSkillCounter = true;

            dataSkillCounter = dataSkill;

            ShowEffectUpgrade();
        }

        public void OnRaiseSkill2()
        {
            ShowEffectUpgrade();
        }

        private void ShowEffectUpgrade()
        {
            if (this.IsAlive)
            {
                ResourceUtils.GetVfx("Effects", "fx_common_troops_upgrade", Vector3.zero, Quaternion.identity,
                    transform);
            }
        }

        private void SetSkinSoldier(int levelTower)
        {
            if (IsGhost)
            {
                UnitSpine.SetSkinSpine($"2004_lv_0{levelTower}_ghost");
            }
            else
            {
                var nameSkin = "2004_lv_0" + levelTower;
                UnitSpine.SetSkinSpine(nameSkin);
            }
        }
    }
}