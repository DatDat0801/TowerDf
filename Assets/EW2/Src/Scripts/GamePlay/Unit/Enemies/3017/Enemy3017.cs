using Invoke;
using UnityEngine;
namespace EW2
{
    public class Enemy3017 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox1;
        public AttackEnemy3017 attack;
        public Skill1Enemy3017 skill1;
        public Skill2Enemy3017 skill2;
        public Enemy3017Spine enemySpine;

        [SerializeField] private string appearVfxName;

        private bool canChangeToPhase1;
        private bool canChangeToPhase2;

        private bool isPhase0;

        private bool isPhase2;


        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3017>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            isPhase0 = true;
            isPhase2 = false;
            canChangeToPhase1 = false;
            canChangeToPhase2 = false;
            CanChooseIsTarget = false;
            enemySpine = (Enemy3017Spine)UnitSpine;
            enemySpine.SetAnimationAppearAndStop();
        }

        protected override void Awake()
        {
            base.Awake();
            
        }

        private void Start()
        {
            CallWave.Instance.startSpawnWave = ComputeCanChangePhase;
            this.Immutable = true;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            // Debug.LogAssertion(this.Immutable);
            if (canChangeToPhase1 || Input.GetKeyDown(KeyCode.J))
            {
                ChangeToPhase1();
            }

            if (isPhase0)
            {
                return;
            }

            if (canChangeToPhase2 || Input.GetKeyDown(KeyCode.K))
            {
                ChangeToPhase2();
            }


            UpdateState(deltaTime);
        }

        private void UpdateState(float deltaTime)
        {
            switch (UnitState.Current)
            {
                case ActionState.None:
                    ExecuteIdle();
                    break;
                case ActionState.Idle:
                    if (isPhase2)
                    {
                        
                        if (this.skill2.CanCastSkill())
                        {
                            skill2.CastSkill();

                        }
                        break;
                    }

                    if (skill1.CanCastSkill())
                    {
                        skill1.CastSkill();
                        break;
                    }

                    if (!IsOutRangeAttackMelee())
                    {
                        //AttackMelee();
                        attackMelee.Execute(deltaTime);
                    }
                    else
                    {
                        ExecuteIdle();
                    }

                    break;
                case ActionState.AttackMelee:
                    if (IsOutRangeAttackMelee())
                    {
                        UnitState.Set(ActionState.Idle);
                    }
                    else
                    {
                        attackMelee.Execute(deltaTime);
                    }

                    break;
            }
        }

        protected override void InitCollider(float timeDelay)
        {
            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            // InvokeProxy.Iinvoke.Invoke(this,
            //     () => {
            //         BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBodyBox);
            //     }, 3f);
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3017Spine(this));

        public void StartPhase1()
        {
            isPhase0 = false;

            //   CanChooseIsTarget = true;
        }


        private void ChangeToPhase1()
        {
            enemySpine.ResumeAppearAnimation();
            var appearVfxClone = ResourceUtils.GetVfx(EffectType.Enemy.ToString(), appearVfxName, transform.position,
                Quaternion.identity);
            appearVfxClone.transform.SetParent(transform);
            canChangeToPhase1 = false;

        }

        private void ChangeToPhase2()
        {
            isPhase2 = true;
            canChangeToPhase2 = false;
            this.Immutable = false;
        }

        private void ComputeCanChangePhase(int currentWave)
        {
            if (currentWave == skill1.skill1Data.waveTrigger)
            {
                canChangeToPhase1 = true;
            }

            if (currentWave == skill2.skill2Data.waveTrigger)
            {
                canChangeToPhase2 = true;
            }
        }


        protected override void InitAction()
        {
            base.InitAction();
            InitSkills();
        }


        private void InitSkills()
        {
            attack.Init(this);
            skill1.Init(this);
            skill2.Init(this);
            // attackMelee.Range = attack.rangeAttackMelee;
        }

        public override void AttackMelee()
        {
            base.AttackMelee();
            attackMelee.onComplete = () => { UnitState.Set(ActionState.Idle); };
        }

        protected override bool IsOutRangeAttackMelee()
        {
            return attack.CalculateAllEnemy().Count == 0;
        }

        public override void CheckEnemyDie()
        {
        }
    }
}