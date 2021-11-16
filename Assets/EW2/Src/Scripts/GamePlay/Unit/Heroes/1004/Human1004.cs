using DG.Tweening;
using Hellmade.Sound;
using Spine;
using UnityEngine;

namespace EW2
{
    public class Human1004 : Dummy
    {
        public Transform pointSpawnBullet;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Human1004Spine(this));

        private HeroData _heroData;

        private HeroData HeroData
        {
            get
            {
                if (this._heroData == null)
                {
                    this._heroData = GameContainer.Instance.Get<UnitDataBase>().Get<HeroData1004>();
                }

                return this._heroData;
            }
        }

        private HeroStatBase HeroStatBase => HeroData.stats[Level - 1];
        public override RPGStatCollection Stats => stats ?? (stats = new HeroStats(this, HeroStatBase));

        private UnitAction _idle;

        private AttackRange _attackRange;

        private UnitAction _skillAttack;

        private UnitAction _skillPassive2;

        private UnitAction _turn;

        private Vector3 _posJumpFlip;

        private Transform _pointFlow;

        private Pet1004 _pet;

        private Vector3 _posTargetPassive;

        public override bool CanControl =>
            IsAlive && UnitState.CanControl() && UnitState.Current != ActionState.SkillPassive2;

        public override void OnEnable()
        {
            UnitState.IsLockState = false;
            UnitState.Set(ActionState.None);

            base.OnEnable();

            Level = 1;
            InitAction();
        }

        public void SetInfo(int heroId, Transform pointFlow, Pet1004 owner)
        {
            SetId(heroId);
            this._pointFlow = pointFlow;
            this._pet = owner;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsAlive) return;

            switch (this._pet.UnitState.Current)
            {
                case ActionState.None:
                    if (UnitState.Current != ActionState.SkillPassive2)
                    {
                        UnitState.Set(ActionState.Idle);
                        this._idle.Execute();
                    }
                    break;
                case ActionState.Move:
                    transform.position = this._pointFlow.transform.position;
                    break;
                case ActionState.AttackMelee:
                    if (UnitState.Current != ActionState.SkillPassive2 && UnitState.Current != ActionState.Idle)
                    {
                        this._idle.Execute();
                    }

                    break;
                case ActionState.Stun:
                    if (UnitState.Current != ActionState.Stun)
                    {
                        UnitState.Set(ActionState.Stun);
                        UnitSpine.Stun();
                    }

                    break;
            }
        }

        public override void Remove()
        {
        }

        protected override void InitAction()
        {
            this._idle = new Idle(this);

            this._attackRange = new AttackRange(this);

            this._skillAttack = new SkillAttack(this);

            this._skillPassive2 = new SkillPassive2(this);

            this._turn = new Turn(this);
        }

        public void ExcuteDie()
        {
            UnitState.Set(ActionState.Die);

            var trackEntry = UnitSpine.Die();
            if (trackEntry != null)
            {
                trackEntry.Complete += PlayDieLoop;
            }
        }

        private void PlayDieLoop(TrackEntry trackentry)
        {
            ((Human1004Spine)UnitSpine).DieLoop();

            trackentry.Complete -= PlayDieLoop;
        }


        public void Revive()
        {
            var trackEntry = ((Human1004Spine)UnitSpine).DieToIdle();
            if (trackEntry != null)
            {
                trackEntry.Complete += ReviveComplete;
            }
        }

        private void ReviveComplete(TrackEntry trackentry)
        {
            UnitState.Set(ActionState.None);
            trackentry.Complete -= ReviveComplete;
        }

        protected override void Awake()
        {
            base.Awake();

            UnitType = UnitType.Hero;
        }

        public override void AttackMelee()
        {
        }

        public void SetAttackRange(Unit enemyTarget)
        {
            if (UnitState.Current == ActionState.SkillPassive2) return;
            AttackRange();
        }

        public override void AttackRange()
        {
            UnitState.Set(ActionState.AttackRange);
        }

        public void ResetTimeRangeAttack()
        {
            this._attackRange.ResetTime();
        }

        public void ExcuteTurn()
        {
            this._turn.onComplete = () => { UnitState.Set(ActionState.None); };
            this._turn.Execute();
        }

        public void SetJumpFlip(Vector3 pos)
        {
            this._posJumpFlip = pos;
            Transform.localScale = this._pet.transform.localScale;
        }

        public void DoFlip()
        {
            transform.DOMove(this._posJumpFlip, 0.2f).OnComplete(() => {
                Transform.localScale = this._pet.transform.localScale;
            });
        }

        public void UseActiveSkill()
        {
            this._skillAttack.Execute();
        }

        public void UsePassive2(Unit enemyTarget)
        {
            if (enemyTarget == null)
            {
                return;
            }

            this._posTargetPassive = enemyTarget.transform.position;

            UnitState.Set(ActionState.SkillPassive2);
            this._skillPassive2.onComplete = () => {
                ((Hero1004PassiveSkill2)this._pet.SkillController.GetSkillPassive(1)).Execute();
                UnitState.Set(ActionState.None);
                this._idle.Execute();
            };
            this._skillPassive2.Execute();
        }

        #region Bullet

        public void SpawnBulletPassive()
        {
            GameObject bullet =
                ResourceUtils.GetUnit("1004_passive_2_acid_bottle", pointSpawnBullet.position,
                    pointSpawnBullet.rotation);
            if (bullet == null)
            {
                return;
            }

            Bullet1004SkillPassive2 control = bullet.GetComponent<Bullet1004SkillPassive2>();
            control.InitBullet1004ActiveSkill(this, this._posTargetPassive, 0.45f);
        }

        #endregion

        #region Impact

        public void SpawnPassiveImpact(Vector3 target)
        {
            var impact = ResourceUtils.GetVfxHero("1004_passive_2_impact", target, Quaternion.identity);
            if (!impact)
            {
                return;
            }

            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_passive_2_impact");
            EazySoundManager.PlaySound(audioClip);
            var control = impact.GetComponent<Hero1004Passive2Impact>();
            var skillData = (Hero1004PassiveSkill2)this._pet.SkillController.GetSkillPassive(1);
            control.InitImpact(this, skillData.dataPassiveCurr);
        }

        #endregion
    }
}
