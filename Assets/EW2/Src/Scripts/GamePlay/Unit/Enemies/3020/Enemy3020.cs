using Invoke;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public enum Phase3020
    {
        Phase1 = 1,
        Phase2 = 2,
        Phase3 = 3
    }

    public class Enemy3020 : EnemyRanger
    {
        [SerializeField] private EnemyTargetCollection rangeDetectAllMap;
        [SerializeField] private EnemyNormalDamageBox normalAttackBox;
        [SerializeField] private Enemy3020Skill4Impact impactSkill4;
        [SerializeField] private Transform pointSpawnBullet;

        public EnemyNormalDamageBox NormalAttackBox => normalAttackBox;

        public EnemyTargetCollection RangeDetectAllMap => rangeDetectAllMap;

        public Enemy3020Skill4Impact ImpactSkill4 => impactSkill4;

        private Phase3020 currentPhase = Phase3020.Phase1;

        public Phase3020 CurrentPhase => currentPhase;

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>().GetStats(Level);
                }

                return enemyData;
            }
        }

        private EnemyData3020 enemyData3020;

        public override UnitSpine UnitSpine => dummySpine = dummySpine ?? new Enemy3020Spine(this);

        private Enemy3020Skill1 enemy3020Skill1;
        private Enemy3020Skill2 skill2;
        private Enemy3020Skill3 skill3;
        private Enemy3020Skill4 skill4;

        private bool gameStart;
        private bool changePhase;

        private SkillPassive1 skillPassive1;
        private SkillPassive2 skillPassive2;
        private SkillPassive3 skillPassive3;
        private SkillPassive4 skillPassive4;

        private bool statusSkipAll;

        protected override void Awake()
        {
            base.Awake();
            CallWave.Instance.startSpawnWave = CheckEndPhase1;
            EventManager.StartListening(GamePlayEvent.OnStartGame, () =>
            {
                gameStart = true;
                ChangePhase(Phase3020.Phase1);
                EventManager.StopListening(GamePlayEvent.OnStartGame, "Boss3020");
            }, "Boss3020");
        }

        private void CheckEndPhase1(int wave)
        {
            if (wave == enemyData3020.GetTriggerEndPhase(GamePlayController.Instance.ModeId + 1).waveTrigger)
            {
                changePhase = true;
                ChangePhase(Phase3020.Phase2);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SetInfo(3020, GamePlayController.Instance.ModeId + 1);
        }

        public override void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
            base.OnDisable();
        }

        public override void SetInfo(int id, int level)
        {
            enemyData3020 = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>();
            base.SetInfo(id, level);
            ((Enemy3020Spine) UnitSpine).UpdateSpineName();
            InitSkill();
            ActivePhase();
            Stats.GetStat(RPGStatType.Health).PropertyChanged += (sender, args) => TriggerHealth();
        }

        protected override void InitAction()
        {
            idle = new Idle(this);

            multiMove = new MultiMove(this);

            singleMove = new SingleMove(this);

            die = new Die(this);

            skillPassive1 = new SkillPassive1(this);

            skillPassive2 = new SkillPassive2(this);

            skillPassive3 = new SkillPassive3(this);

            skillPassive4 = new SkillPassive4(this);

            UnitState.Set(ActionState.None);
        }

        private void InitSkill()
        {
            enemy3020Skill1 = new Enemy3020Skill1();
            enemy3020Skill1.Init(this);

            skill2 = new Enemy3020Skill2();
            skill2.Init(this);

            skill3 = new Enemy3020Skill3();
            skill3.Init(this);

            skill4 = new Enemy3020Skill4();
            skill4.Init(this);
        }

        private void ActivePhase()
        {
            if (currentPhase == Phase3020.Phase1)
            {
                rangeDetectAllMap.gameObject.SetActive(true);
                rangeSearchTarget.gameObject.SetActive(false);
                normalAttackBox.gameObject.SetActive(false);
                blockCollider.gameObject.SetActive(false);
                BodyCollider.gameObject.SetActive(false);
            }
            else if (currentPhase == Phase3020.Phase2)
            {
                ((Enemy3020Spine) UnitSpine).UpdateSpineName();
                attackMelee = new AttackMelee(this, EnemyData.detectMeleeAttack);
                attackRange = new AttackRange(this);
                rangeDetectAllMap.gameObject.SetActive(false);
                normalAttackBox.gameObject.SetActive(true);
                blockCollider.gameObject.SetActive(true);
                BodyCollider.gameObject.SetActive(true);

                var line = GamePlayController.Instance.SpawnController.MapController.lines[3];
                var paths = line.GetPathWaypoints();
                if (paths != null && paths.Count > 0)
                {
                    LineController = line;
                    MoveToEndPoint(paths);
                }
            }
            else
            {
                ((Enemy3020Spine) UnitSpine).UpdateSpineName();
                normalAttackBox.gameObject.SetActive(true);
                blockCollider.gameObject.SetActive(true);
                rangeSearchTarget.gameObject.SetActive(true);
                BodyCollider.gameObject.SetActive(true);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsAlive || IsCompleteEndPoint) return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (CurrentPhase == Phase3020.Phase1)
                {
                    gameStart = true;
                    changePhase = true;
                    ChangePhase(Phase3020.Phase2);
                }
                else if (CurrentPhase == Phase3020.Phase2)
                {
                    UnitState.Set(ActionState.None);
                    UnitState.Set(ActionState.Die);
                    changePhase = true;
                    ChangePhase(Phase3020.Phase3);
                }
            }

            if (gameStart && !changePhase && !statusSkipAll)
                UpdateState(deltaTime);

            if (changePhase && currentPhase == Phase3020.Phase2) return;

            UpdateAction(deltaTime);
        }

        private void UpdateAction(float deltaTime)
        {
            if (!statusSkipAll)
            {
                switch (UnitState.Current)
                {
                    case ActionState.None:
                        ExecuteNoneState();
                        break;
                    case ActionState.Move:
                        if (isMovingToEndPoint)
                        {
                            multiMove.Execute();

                            if (currentPhase == Phase3020.Phase3)
                            {
                                if (IsInRangeAttackRange())
                                {
                                    AttackRange();
                                }
                            }
                        }
                        else
                        {
                            if (IsOutRangeAttackMelee())
                            {
                                singleMove.Execute();
                            }
                            else
                            {
                                AttackMelee();
                            }
                        }

                        break;
                    case ActionState.AttackMelee:
                        if (IsValidTarget())
                        {
                            if (IsOutRangeAttackMelee())
                            {
                                blockCollider.MoveToBlockTarget();
                            }
                            else
                            {
                                Flip(target.Transform.position.x);
                                attackMelee.Execute(deltaTime);
                            }
                        }
                        else
                        {
                            ResetTarget();
                        }

                        break;
                    case ActionState.AttackRange:
                        if (IsValidTarget())
                        {
                            Flip(target.Transform.position.x);

                            attackRange.Execute(deltaTime);
                        }
                        else
                        {
                            ResetTarget();
                        }

                        break;
                    case ActionState.Die:
                    case ActionState.Stun:
                        break;
                }
            }
            else
            {
                multiMove.Execute();
            }
        }

        private void UpdateState(float deltaTime)
        {
            if (currentPhase == Phase3020.Phase1)
            {
                if (enemy3020Skill1 != null)
                {
                    if (enemy3020Skill1.CanCastSkill())
                    {
                        if (changePhase) return;
                        skillPassive1.onComplete = () => { UnitState.Set(ActionState.None); };
                        skillPassive1.Execute();
                        enemy3020Skill1.CastSkill();
                    }
                }
            }
            else if (currentPhase == Phase3020.Phase2)
            {
                if (skill2 != null && skill2.CanCastSkill())
                {
                    skillPassive2.onComplete = () =>
                    {
                        if (changePhase) return;
                        Flip(multiMove.currentTarget.x);
                        UnitState.Set(ActionState.Move);
                        UnitSpine.Move();
                    };
                    skillPassive2.Execute();
                    skill2.CastSkill();
                }
                else if (skill3 != null && skill3.CanCastSkill())
                {
                    skillPassive3.onComplete = () =>
                    {
                        if (changePhase) return;
                        Flip(multiMove.currentTarget.x);
                        UnitState.Set(ActionState.Move);
                        UnitSpine.Move();
                    };
                    skillPassive3.Execute();
                    skill3.CastSkill();
                }
            }
            else if (currentPhase == Phase3020.Phase3)
            {
                if (skill4 != null && skill4.CanCastSkill())
                {
                    skillPassive4.onComplete = () =>
                    {
                        if (changePhase) return;
                        statusSkipAll = true;
                        InvokeProxy.Iinvoke.Invoke(this, ResetStatusSkipAll, skill4.timeStun);
                        Flip(multiMove.currentTarget.x);
                        UnitState.Set(ActionState.Move);
                        UnitSpine.Move();
                    };
                    skillPassive4.Execute();
                    skill4.CastSkill();
                }
            }
        }

        private void ChangePhase(Phase3020 phase)
        {
            if (phase == Phase3020.Phase1)
            {
                GamePlayController.Instance.SpawnController.AddCountEnemy();
                enemy3020Skill1.StatCooldown();
            }
            else if (phase == Phase3020.Phase2)
            {
                var posBack = transform.position;
                posBack.x -= 3f;

                singleMove.onFinish = () =>
                {
                    currentPhase = Phase3020.Phase2;
                    ActivePhase();
                    changePhase = false;
                    skill2.StatCooldown();
                    skill3.StatCooldown();
                };

                singleMove.SetTarget(posBack);
                UnitState.Set(ActionState.Move);
            }
            else
            {
                HideAllBoxCollider();

                HealthBar.Hide(0);
                ChangePhase3();
            }
        }

        void ChangePhase3()
        {
            InvokeProxy.Iinvoke.Invoke(this, () => { die.Execute(); }, 0.8f);
            InvokeProxy.Iinvoke.Invoke(this, () =>
            {
                currentPhase = Phase3020.Phase3;
                ActivePhase();
                HealthBar.Show();
                changePhase = false;
                UnitState.Set(ActionState.None);
                skill4.StatCooldown();
                UnitState.Set(ActionState.Move);
                UnitSpine.Move();
            }, 2.4f);
        }

        private void TriggerHealth()
        {
            if (changePhase || currentPhase == Phase3020.Phase3) return;

            var health = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            if (health.CalculateCurrentPercent() <=
                enemyData3020.GetTriggerEndPhase(GamePlayController.Instance.ModeId + 1).percentHp)
            {
                if (!changePhase)
                {
                    UnitState.Set(ActionState.None);
                    UnitState.Set(ActionState.Die);
                    changePhase = true;
                    ChangePhase(Phase3020.Phase3);
                }
            }
        }

        private void HideAllBoxCollider()
        {
            rangeDetectAllMap.gameObject.SetActive(false);
            rangeSearchTarget.gameObject.SetActive(false);
            normalAttackBox.gameObject.SetActive(false);
            blockCollider.gameObject.SetActive(false);
            BodyCollider.gameObject.SetActive(false);
        }

        public void SpawnBullet()
        {
            var goBullet = ResourceUtils.GetUnit("bullet_3020");

            if (goBullet)
            {
                goBullet.transform.position = pointSpawnBullet.position;
                goBullet.GetComponent<Bullet3020>().InitBullet(this, target);
            }
        }

        private void ResetStatusSkipAll()
        {
            statusSkipAll = false;
        }
    }
}