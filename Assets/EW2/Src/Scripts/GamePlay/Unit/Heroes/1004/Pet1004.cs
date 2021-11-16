using System;
using DG.Tweening;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using Spine;
using Unity.Mathematics;
using UnityEngine;

namespace EW2
{
    public class Pet1004 : HeroBase
    {
        private const float TIME_DELAY_RELAX = 9f;

        public HeroNormalDamageBox normalAttackBox;
        public Transform pointSpawnBullet;
        public Transform posHumanLeft;
        public Transform posHumanRight;
        public float speedGoOut;
        public float speedGoIn;
        public Hero1004Passive3Collider passive3Collider;

        private Human1004 _human1004;
        private float _timeTriggerRelax = 0;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Pet1004Spine(this));

        public override HeroData HeroData
        {
            get
            {
                if (heroData == null)
                {
                    heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1004>();
                }

                return heroData;
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));

        private HeroData1004.PoisonStatus _poisonStatusData;

        private Vector3 _targetActiveSkill;

        private GameObject _smoke;

        private Turn _turn;

        //skill
        private Hero1004ActiveSkill _activeSkill;
        private Hero1004PassiveSkill1 _passiveSkill1;
        private Hero1004PassiveSkill2 _passiveSkill2;
        private Hero1004PassiveSkill3 _passiveSkill3;

        protected override void Awake()
        {
            base.Awake();
            SetInfo(1004);
            SpawnHuman();
            SetRangeAttack();
            SearchTarget.SetBlockNumber(HeroStatBase.blockEnemy);
            this._poisonStatusData = ((HeroData1004)HeroData).poisonStatuses[Level - 1];
            InitSkill();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            InitCustomStats();
        }
        
        private void SpawnHuman()
        {
            var human = ResourceUtils.GetUnit("1004_human",null,false);
            if (human)
            {
                human.transform.position = posHumanLeft.position;
                this._human1004 = human.GetComponent<Human1004>();
                this._human1004.SetInfo(1004, posHumanLeft, this);
            }
        }
        private void InitSkill()
        {
            var heroData1004 = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1004>();

            this._activeSkill = new Hero1004ActiveSkill(this, heroData1004.active);
            this._activeSkill.Init();
            SkillController.SetSkillActive(this._activeSkill);

            this._passiveSkill1 = new Hero1004PassiveSkill1(this, heroData1004.passive1);
            this._passiveSkill1.Init();
            SkillController.AddSkillPassive(this._passiveSkill1);

            this._passiveSkill2 = new Hero1004PassiveSkill2(this, heroData1004.passive2);
            this._passiveSkill2.Init();
            SkillController.AddSkillPassive(this._passiveSkill2);

            this._passiveSkill3 = new Hero1004PassiveSkill3(this, heroData1004.passive3);
            this._passiveSkill3.Init();
            SkillController.AddSkillPassive(this._passiveSkill3);
            

        }

        void InitCustomStats()
        {
            //add custom stat
            if (Stats is HeroStats heroStats)
            {
                var skillData = ((Hero1004ActiveSkill)SkillController.SkillActive).SkillData;
                if (skillData != null)
                    heroStats.AddCustomCooldownStat(skillData.cooldown);
            }
        }

        #region Skill Active

        //  Skill active
        public override float GetCoolDownTime()
        {
            var timeCooldown = ((Hero1004ActiveSkill)SkillController.SkillActive).SkillData.cooldown;
            timeCooldown -= timeCooldown * Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
            return timeCooldown;
        }

        public override void ActiveSkillToTarget(Vector3 target, Action callbackCooldown)
        {
            base.ActiveSkillToTarget(target, callbackCooldown);

            if (!CanControl)
                return;

            move.Stop();
            SetIdleState();
            this._targetActiveSkill = target;
            UseSkillActive();
            callbackCooldown?.Invoke();
        }

        public override void UseSkillActive()
        {
            if (CanControl == false)
                return;

            skillAttack.onComplete = () => {
                Debug.LogWarning("done skill active");
                SetIdleState();
                CheckActionCallback();
            };

            if (CheckCanFlip(this._targetActiveSkill.x))
            {
                FlipInBattle(this._targetActiveSkill.x, () => {
                    ExcuteAnimActiveSkill();
                    base.UseSkillActive();
                });
            }
            else
            {
                ExcuteAnimActiveSkill();
                base.UseSkillActive();
            }
        }

        private void ExcuteAnimActiveSkill()
        {
            this._human1004.UseActiveSkill();
            skillAttack.Execute();
        }

        #endregion

        public override void OnUpdate(float deltaTime)
        {
            if (this == null || !IsAlive) return;

            switch (UnitState.Current)
            {
                case ActionState.None:
                    idle.Execute();
                    RegenHp.Execute();
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
                            if (CheckCanFlip(target1.Transform.position.x))
                            {
                                FlipInBattle(target1.Transform.position.x, () => { SetStateAttackMelee(); });
                            }
                            else
                            {
                                attackMelee.Execute(deltaTime);
                            }

                            RegenHp.Stop();
                        }
                        else
                        {
                            SetIdleState();
                        }
                    }

                    break;
                case ActionState.AttackRange:
                    var target = searchTarget.target.Value;
                    if (target && target.IsAlive)
                    {
                        if (CheckCanFlip(target.Transform.position.x))
                        {
                            FlipInBattle(target.Transform.position.x, () => {
                                attackRange.ResetTime();
                                this._human1004.ResetTimeRangeAttack();
                                SetStateAttackRange();
                            });
                        }
                        else
                        {
                            attackRange.Execute(deltaTime);
                        }

                        RegenHp.Stop();
                    }
                    else
                    {
                        SetIdleState();
                    }

                    break;
                case ActionState.Die:
                    break;
                case ActionState.Idle:
                    if (this._human1004.UnitState.Current != ActionState.SkillPassive2)
                        CheckActivePassive3();
                    HandleRelaxStatus(deltaTime);
                    break;
            }
        }

        public override void OnSetState(ActionState state)
        {
            base.OnSetState(state);
            if (state.Equals(ActionState.Move))
            {
                this._human1004.UnitState.Set(ActionState.Move);
                this._human1004.UnitSpine.Move();
            }
        }

        protected override void OnTargetChange(object sender, EventArgs args)
        {
            if (IsAlive == false)
            {
                print($"Hero {name} was died");
                return;
            }

            Debug.LogWarning("targetType: " + searchTarget.targetAttackType.Value);

            switch (searchTarget.targetAttackType.Value)
            {
                case AttackType.Melee:
                    InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    AttackMelee();
                    break;
                case AttackType.Range:
                    InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
                    AttackRange();
                    break;
                case AttackType.None:
                    if (IsAlive)
                    {
                        InvokeProxy.Iinvoke.Invoke(this, MoveToPosDefault, 2f);
                    }

                    if (UnitState.Current != ActionState.UseSkill)
                    {
                        this.move.Stop();
                        SetIdleState();
                    }

                    break;
            }
        }

        private void SetIdleState()
        {
            this._timeTriggerRelax = 0;
            UnitState.Set(ActionState.None);
        }

        protected override void InitAction()
        {
            base.InitAction();

            attackRange = new AttackRange1004(this, this._human1004);
            this._turn = new Turn(this);

            attackMelee.onComplete = CheckActiveSkillPassive;
            attackRange.onComplete = CheckActiveSkillPassive;
        }

        public override void AttackMelee()
        {
            if (!this.CanControl || this.searchTarget.target.Value == null)
            {
                return;
            }

            this.move.Stop();

            var posXTarget = this.searchTarget.target.Value.Transform.position.x;

            if (CheckCanFlip(posXTarget))
            {
                FlipInBattle(posXTarget, SetStateAttackMelee);
            }
            else
            {
                SetStateAttackMelee();
            }
        }

        private void SetStateAttackMelee()
        {
            attackMelee.ResetTime();
            UnitState.Set(ActionState.AttackMelee);
        }


        public override void AttackRange()
        {
            if (CanControl && searchTarget.target.Value != null)
            {
                var posXTarget = searchTarget.target.Value.Transform.position.x;

                if (CheckCanFlip(posXTarget))
                {
                    FlipInBattle(posXTarget, SetStateAttackRange);
                }
                else
                {
                    SetStateAttackRange();
                }
            }
        }

        private void SetStateAttackRange()
        {
            UnitState.Set(ActionState.AttackRange);
            if (this._human1004.UnitState.Current != ActionState.SkillPassive2)
            {
                this._human1004.SetAttackRange(searchTarget.target.Value);
            }
        }

        private void CheckActiveSkillPassive()
        {
            CheckActivePassive2();

            if (this._human1004.UnitState.Current != ActionState.SkillPassive2)
            {
                CheckActivePassive3();
            }
        }

        private void CheckActivePassive2()
        {
            if (!CanControl || this._passiveSkill2.Level <= 0 ||
                this._human1004.UnitState.Current == ActionState.SkillPassive2)
                return;

            if (this._passiveSkill2 != null)
            {
                if (!this._passiveSkill2.IsActive())
                    this._passiveSkill2.CheckActivePassive();

                if (this._passiveSkill2.IsActive())
                {
                    this._human1004.UsePassive2(searchTarget.target.Value);
                }
            }
        }

        private void CheckActivePassive3()
        {
            if (!CanControl || this._passiveSkill3.Level <= 0 || UnitState.Current == ActionState.SkillPassive3)
                return;

            if (this._passiveSkill3 == null)
            {
                return;
            }

            if (!this._passiveSkill3.IsReady() || this.passive3Collider.Targets.Count <= 1)
            {
                return;
            }

            SetIdleState();
            this._human1004.UnitState.Set(ActionState.None);

            if (CheckCanFlip(this.passive3Collider.Targets[1].transform.position.x))
                FlipInBattle(this.passive3Collider.Targets[1].transform.position.x, ActivePassive3);
            else
                ActivePassive3();
        }

        private void ActivePassive3()
        {
            UnitState.Set(ActionState.SkillPassive3);
            this._passiveSkill3.Execute();
            skillPassive3.onComplete = () => {
                CheckActionCallback();
            };
            skillPassive3.Execute();
        }

        #region Move

        public override void MoveToPosDefault()
        {
            if (Vector3.Distance(transform.position, posDefault) < 0.3f) return;

            this._human1004.UnitSpine.Move();

            Move(posDefault, b => {
                SetIdleState();

                SetActiveSearchTarget(true);
            });
        }

        public override void TouchMove(Vector3 target)
        {
            if (!CanControl)
                return;

            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);

            posDefault = target;

            Tracking.Move();

            this._human1004.UnitState.Set(ActionState.Move);
            this._human1004.UnitSpine.Move();

            SetActiveSearchTarget(false);

            Move(target, b => {
                SetIdleState();
                SetActiveSearchTarget(true);
            });
        }

        public override void Move(Vector3 target, Action<bool> callback)
        {
            var timeDelay = 0f;

            if (CheckCanFlip(target.x))
            {
                timeDelay = 0.4f;
                FlipNormal(target.x);
            }

            InvokeProxy.Iinvoke.Invoke(this, () => {
                UnitState.Set(ActionState.Move);

                UnitSpine.Move();

                this._human1004.UnitState.Set(ActionState.Move);

                this._human1004.UnitSpine.Move();

                move.SetDestination(target, callback);

                RegenHp.Stop();
            }, timeDelay);
        }

        #endregion

        #region Relax

        private void HandleRelaxStatus(float timeUpdate)
        {
            this._timeTriggerRelax += timeUpdate;
            if (this._timeTriggerRelax >= TIME_DELAY_RELAX)
            {
                this._timeTriggerRelax = 0;
                ((Human1004Spine)this._human1004.UnitSpine).IdleRelax();
                var trackEntry = ((Pet1004Spine)UnitSpine).IdleRelax();
                if (trackEntry != null)
                {
                    trackEntry.Complete += RelaxComplete;
                }
            }
        }

        private void RelaxComplete(TrackEntry trackentry)
        {
            SetIdleState();
            trackentry.Complete -= RelaxComplete;
        }

        #endregion

        #region Die

        public override void Remove()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_die");
            EazySoundManager.PlaySound(audioClip);

            move.Stop();
            
            Stats.ClearStatModifiers();

            StatusController.RemoveAll();

            SetActiveSearchTarget(false);

            InvokeProxy.Iinvoke.CancelInvoke(this, MoveToPosDefault);
            
            MoveOutSide();
        }

        private void MoveOutSide()
        {
            Vector3 posTarget;

            var position = transform.position;

            posTarget = transform.localScale.x > 0
                ? new Vector3(15f, position.y, position.z)
                : new Vector3(-15f, position.y, position.z);

            OnDie();

            die.Execute();

            this._human1004.ExcuteDie();

            posDefault = transform.position;

            var currSpeed = 2f;

            var timeMove = (posTarget - position).magnitude / (currSpeed * speedGoOut);

            this._smoke =
                ResourceUtils.GetVfx("Hero", "1004_pet_move_die", Vector3.zero, quaternion.identity, transform);
            this._smoke.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);

            transform.DOMove(posTarget, timeMove).OnComplete(() => {
                transform.position = new Vector3(-transform.position.x, position.y, position.z);
                this._smoke.SetActive(false);
            });
        }

        public override void OnDie()
        {
            if (healthBar)
                healthBar.gameObject.SetActive(false);

            if (heroButton)
                heroButton.Deactive();

            if (skillButton)
                skillButton.Deactive();

            RegenHp.Stop();
        }

        #endregion

        #region Flip

        private void FlipInBattle(float positionX, Action flipComplete)
        {
            this._turn.onComplete = () => { flipComplete?.Invoke(); };
            Flip(positionX);
        }

        private void FlipNormal(float positionX)
        {
            this._turn.onComplete = TurnComplete;
            Flip(positionX);
        }

        public override void Flip(float positionX)
        {
            if (this._human1004.UnitState.Current == ActionState.SkillPassive2)
            {
                return;
            }

            move.Stop();
            //
            this._human1004.SetJumpFlip(posHumanRight.position);
            this._human1004.ExcuteTurn();
            //
            this._turn.Execute();
        }

        private void TurnComplete()
        {
            SetIdleState();
        }

        public void DoFlip()
        {
            var localScale = Transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            Transform.localScale = localScale;
            FlipComponent();
        }

        private bool CheckCanFlip(float positionX)
        {
            var flip = Transform.position.x > positionX;
            var localScale = this.Transform.localScale;
            return (localScale.x > 0 && flip) || (localScale.x < 0 && flip == false);
        }

        #endregion

        #region Revive

        public override void Revive()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_revive");
            EazySoundManager.PlaySound(audioClip);

            this._human1004.Revive();

            gameObject.SetActive(true);

            var currSpeed = 2f;

            var timeMove = (posDefault - transform.position).magnitude / (currSpeed * speedGoIn);

            this._smoke.SetActive(true);

            transform.DOMove(posDefault, timeMove).OnComplete(() => {
                SetIdleState();
                SetActiveSearchTarget(true);
                LeanPool.Despawn(this._smoke);
                this._smoke = null;
            });

            base.Revive();
        }

        #endregion

        #region Bullet

        public void SpawnBulletRangeAttack()
        {
            if (!this.SearchTarget.HasTarget)
            {
                return;
            }

            var target = this.SearchTarget.target.Value;

            if (target == null)
            {
                return;
            }

            ResourceUtils.GetVfx("Hero", "1004_attack_range_muzzle", this.pointSpawnBullet.localPosition,
                Quaternion.identity, this.transform);

            GameObject bullet = ResourceUtils.GetUnit("1004_range_bullet", this.pointSpawnBullet.position,
                this.pointSpawnBullet.rotation);
            if (bullet == null)
            {
                return;
            }

            Bullet1004 control = bullet.GetComponent<Bullet1004>();
            control.InitBullet1004(this, target, 0.45f, this._poisonStatusData.timeLife,
                this._poisonStatusData.hpPerSecond);
        }

        public void SpawnBulletActive()
        {
            GameObject bullet =
                ResourceUtils.GetUnit("1004_active_ball", pointSpawnBullet.position, pointSpawnBullet.rotation);
            if (bullet == null)
            {
                return;
            }

            Bullet1004ActiveSkill control = bullet.GetComponent<Bullet1004ActiveSkill>();
            control.InitBullet1004ActiveSkill(this, this._targetActiveSkill, 0.45f);
        }

        public void SpawnBulletPassive3()
        {
            foreach (var heroTarget in this.passive3Collider.Targets)
            {
                if (this.GetInstanceID() == heroTarget.GetInstanceID())
                {
                    continue;
                }

                var bullet = ResourceUtils.GetUnit("1004_passive_3_ball", this.pointSpawnBullet.position,
                    this.pointSpawnBullet.rotation);

                if (bullet == null)
                {
                    continue;
                }

                var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_passive_3_throw");
                EazySoundManager.PlaySound(audioClip);
                Bullet1004SkillPassive3 control = bullet.GetComponent<Bullet1004SkillPassive3>();
                control.InitBullet1004ActiveSkill(this, heroTarget, 0.45f);
            }
        }

        #endregion

        #region Effect

        public void SpawnEffectMeleeAttack()
        {
            ResourceUtils.GetVfxHero("1004_attack_melee_impact", new Vector3(0.6f, 0.25f, 0f), Quaternion.identity,
                transform);
        }

        public void SpawnActiveImpact(Vector3 target)
        {
            var impact =
                ResourceUtils.GetVfxHero("1004_active_impact", target, Quaternion.identity); //1004_active_ground_acid
            if (!impact)
            {
                return;
            }

            var groundAcid = ResourceUtils.GetVfxHero("1004_active_ground_acid", target, Quaternion.identity);
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_active_skill_impact");
            EazySoundManager.PlaySound(audioClip);

            if (!groundAcid) return;

            var control = groundAcid.GetComponent<Hero1004ActiveImpact>();
            control.InitImpact(this, this._activeSkill.SkillData);
            LeanPool.Despawn(groundAcid, this._activeSkill.SkillData.timeLife);
        }

        public void SpawnPassive3Impact(Vector3 target, HeroBase heroTarget)
        {
            ResourceUtils.GetVfxHero("1004_passive_3_impact_yellow", target, Quaternion.identity);
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_passive_3_impact");
            EazySoundManager.PlaySound(audioClip);
            AddStatusPassive3(heroTarget);
        }

        private void AddStatusPassive3(Unit heroTarget)
        {
            if (!heroTarget.IsAlive)
            {
                return;
            }

            //hp
            var healOverTime = new HealOverTime(new StatusOverTimeConfig() {
                statusType = StatusType.HealInstantOverTime,
                creator = this,
                owner = heroTarget,
                lifeTime = this._passiveSkill3.DataPassiveCurr.timeLife,
                intervalTime = 1,
                baseValue = this._passiveSkill3.DataPassiveCurr.regenHpPerSecond
            });

            heroTarget.StatusController.AddStatus(healOverTime);

            //atk speed
            var attackSpeedAttribute = heroTarget.Stats.GetStat<RPGAttribute>(RPGStatType.AttackSpeed);

            var statusOverTimeConfig = new StatusOverTimeConfig() {
                creator = this,
                owner = heroTarget,
                lifeTime = this._passiveSkill3.DataPassiveCurr.timeLife,
            };
            var modifierAttackSpeed = new RPGStatModifier(attackSpeedAttribute,
                this._passiveSkill3.DataPassiveCurr.modifierType,
                this._passiveSkill3.DataPassiveCurr.ratioIncreaseAtkSpeed, false, this, heroTarget);
            var modifierAttackSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierAttackSpeed);

            heroTarget.StatusController.AddStatus(modifierAttackSpeedOverTime);
        }

        #endregion
    }
}
