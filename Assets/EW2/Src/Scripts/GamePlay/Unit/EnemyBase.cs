using Cysharp.Threading.Tasks;
using Invoke;
using Lean.Pool;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public abstract class EnemyBase : Dummy
    {
        protected EnemyStatBase enemyData;

        //[ShowInInspector]
        public virtual EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().GetEnemyById(Id).GetStats(Level);
                }

                return enemyData;
            }
        }
        
        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new EnemyStats(this, EnemyData);
                }

                return stats;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EnemyBlockCollider blockCollider;

        public LineController LineController { get; set; }

        protected BodyCollider bodyCollider;

        protected Renderer _renderer;

        public Renderer Renderer
        {
            get
            {
                if (this._renderer == null)
                    this._renderer = GetComponent<Renderer>();

                return this._renderer;
            }
        }

        public BodyCollider BodyCollider
        {
            get
            {
                if (bodyCollider == null)
                    bodyCollider = GetComponentInChildren<BodyCollider>();

                return bodyCollider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Transform endPoint;

        public MoveType MoveType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public TraitEnemyType TraitType { get; protected set; }

        [System.NonSerialized] public Dummy target;

        /// <summary>
        /// 
        /// </summary>
        protected Idle idle;

        /// <summary>
        /// 
        /// </summary>
        protected SingleMove singleMove;

        /// <summary>
        /// 
        /// </summary>
        protected MultiMove multiMove;

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
        protected Die die;

        /// <summary>
        /// is moving to target
        /// </summary>
        protected bool isMovingToEndPoint;

        protected bool isMovingToStartPoint;

        protected Coroutine effectDieCoroutine;

        public int GoldDrop { get; set; }

        public bool IsCompleteEndPoint { get; set; }

        protected override void Awake()
        {
            base.Awake();

            UnitType = UnitType.Enemy;

            if (endPoint == null)
                endPoint = FindObjectOfType<EndPointController>()?.transform;
        }

        public virtual void SetInfo(int id, int level)
        {
            SetId(id);

            SetLevel(level);

            MoveType = EnemyData.moveType;

            TraitType = EnemyData.traitType;

            DamageType = EnemyData.damageType;

            Stats.ConfigureStats();

            InitAction();

            if (HealthBar)
            {
                HealthBar.SetHealthBar(Stats.GetStat<HealthPoint>(RPGStatType.Health));
            }

            IsCompleteEndPoint = false;
        }

        private void SetLevel(int level)
        {
            // depend on campaign mode
            this.Level = level;
        }

        protected override void InitAction()
        {
            //make sure reset on enemy die with Stun state
            UnitState.IsLockState = false;
            RemoveColdOverrideMaterial();
            UnitState.Set(ActionState.None);

            idle = new Idle(this);

            multiMove = new MultiMove(this);

            singleMove = new SingleMove(this);

            die = new Die(this);

            if (TraitType != TraitEnemyType.Trickster)
            {
                attackMelee = new AttackMelee(this);

                if (TraitType == TraitEnemyType.Skirmisher)
                {
                    attackRange = new AttackRange(this);
                }
            }
        }

        public void SetBlockCollider(bool isActive)
        {
            if (blockCollider)
                blockCollider.gameObject.SetActive(isActive);
            else
            {
                throw new Exception("enemy doesn't have block collider");
            }
        }
        

        //public ActionState debugState;

        public override void OnUpdate(float deltaTime)
        {
            //debugState = UnitState.Current;
            if (!IsAlive) return;

            switch (UnitState.Current)
            {
                case ActionState.None:
                    ExecuteNoneState();
                    break;
                case ActionState.Move:
                    if (isMovingToEndPoint)
                    {
                        multiMove.Execute();

                        if (TraitType == TraitEnemyType.Skirmisher)
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
                            if (attackRange != null && TraitType == TraitEnemyType.Skirmisher)
                            {
                                AttackRange();
                            }
                            else
                            {
                                singleMove.Execute();
                            }
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
                    break;
                case ActionState.Terrify:
                    if (isMovingToStartPoint)
                        multiMove.Execute();
                    break;
            }
        }

        protected bool IsValidTarget()
        {
            return target != null && target.IsAlive;
        }

        protected virtual void ExecuteIdle()
        {
            idle.Execute();
        }

        protected virtual void ExecuteNoneState()
        {
            idle.Execute();
        }

        protected virtual bool IsOutRangeAttackMelee()
        {
            if (this.target is DefensivePointBase)
            {
                //Due to Defensive Point too big, so increase attack melee rage a bit 
                return attackMelee == null || attackMelee.Range + 1.4f < blockCollider.Distance();
            }
            return attackMelee == null || attackMelee.Range < blockCollider.Distance();
        }

        protected virtual bool IsInRangeAttackRange()
        {
            return false;
        }

        protected void ResetTarget()
        {
            target = null;
            idle.Execute();
            MoveToEndPoint();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SetBlockCollider(true);

            var timeDelayReInit = 3f;

            InitCollider(timeDelayReInit);
        }

        public override void OnDisable()
        {
            SetBlockCollider(false);

            if (gameObject != null && effectDieCoroutine != null && CoroutineUtils.Instance != null)
                CoroutineUtils.Instance.StopCoroutine(effectDieCoroutine);

            effectDieCoroutine = null;

            base.OnDisable();
        }

        protected virtual void InitCollider(float timeDelay)
        {
            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            InvokeProxy.Iinvoke.Invoke(this,
                () => {
                    blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBlock);

                    BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBodyBox);
                }, timeDelay);
        }

        public virtual void EnableColliderImmediate()
        {
            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBlock);

            BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyBodyBox);
        }

        public override void AttackMelee()
        {
            if (UnitState.IsStatusCC()) return;

            attackMelee.ResetTime();

            UnitState.Set(ActionState.AttackMelee);
        }

        public override void AttackRange()
        {
            if (UnitState.IsStatusCC()) return;

            attackRange.ResetTime();

            UnitState.Set(ActionState.AttackRange);
        }

        public void MoveToBlockTarget(Transform target)
        {
            if (!IsAlive)
            {
                return;
            }

            isMovingToEndPoint = false;

            if (transform)
            {
                MoveToTarget(target);
            }
            else
            {
                throw new Exception("target is null");
            }
        }

        public void MoveToEndPoint(List<Vector3> pointList = null)
        {
            if (MoveType == MoveType.None || !IsAlive)
            {
                return;
            }

            isMovingToEndPoint = true;

            UnitState.Set(ActionState.Move);

            // new path to end point
            if (pointList != null)
            {
                multiMove.SetTarget(pointList);
            }
            else
            {
                Flip(multiMove.currentTarget.x);

                UnitSpine.Move();
            }
        }

        public void MoveToTarget(Transform target)
        {
            if (!IsAlive)
            {
                return;
            }

            UnitState.Set(ActionState.Move);

            singleMove.SetTarget(target);

            singleMove.onFinish = () => { UnitState.Set(ActionState.None); };
        }

        public void MoveToStartPoint()
        {
            if (MoveType == MoveType.None || !IsAlive)
            {
                return;
            }

            UnitSpine.Idle();

            List<Vector3> pointList = LineController.CalculateGoBackWayPoints(transform.position);

            // new path to end point
            if (pointList != null)
            {
                multiMove.SetTarget(pointList);
            }
        }

        public void EnableMoveToStartPoint(bool enable)
        {
            this.isMovingToStartPoint = enable;
        }

        public void CheckActionAfterStatus()
        {
            if (this.target)
            {
                MoveToBlockTarget(this.target.transform);

                var points = LineController.CalculateRemainPathWayPoints(transform.position);
                multiMove.SetTarget(points);
            }
            else
            {
                var points = LineController.CalculateRemainPathWayPoints(transform.position);
                MoveToEndPoint(points);
            }
        }

        public override void Remove()
        {
            StatusController.RemoveAll();

            Stats.ClearStatModifiers();

            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            GetComponentInChildren<BodyCollider>().gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            SetBlockCollider(false);

            if (!IsCompleteEndPoint)
            {
                die.onComplete = () => { effectDieCoroutine = CoroutineUtils.Instance.StartCoroutine(EffectDie()); };

                die.Execute();
            }
            else
            {
                blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

                BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

                idle.Execute();

                LeanPool.Despawn(gameObject);
            }
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);

            if (!IsAlive)
            {
                CheckEnemyDie();
                GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, GoldDrop);
                EventManager.EmitEventData(GamePlayEvent.OnEnemyKill, new int[] {Id, damageInfo.creator.Id});
                Debug.Log($"<color=pink>{name} was killed by {damageInfo.creator}, received {GoldDrop} gold</color>");
            }
        }

        public virtual void CheckEnemyDie()
        {
            SpawnControllerBase.spawnedEnemies.Remove(Id);
            EventManager.EmitEventData(GamePlayEvent.OnEnemyDie, this.Id);
        }

        public float DistanceToEndPoint()
        {
            if (endPoint)
            {
                return Vector2.Distance(Transform.position, endPoint.position);
            }

            return Single.MaxValue;
        }

        public float DistanceToTarget()
        {
            if (target)
            {
                return Vector2.Distance(transform.position, target.transform.position);
            }

            return float.MaxValue;
            //throw new Exception("target is null");
        }

        public Vector2 GetFuturePosition(float time)
        {
            if (UnitState.Current == ActionState.Move)
            {
                float distance = (time * singleMove.Speed);
                float temp = distance;
                Vector3 tempPoint = transform.position;

                if (multiMove.TargetList.Count > 0)
                {
                    var pos = tempPoint + (multiMove.TargetList[0] - tempPoint).normalized * temp;
                    return pos;
                }
                else
                {
                    var pos = tempPoint + tempPoint.normalized * temp;
                    return pos;
                }
            }

            return transform.position;
        }

        public SingleMove GetSingleMove()
        {
            return this.singleMove;
        }

        public MultiMove GetMultiMove()
        {
            return this.multiMove;
        }

        /// <summary>
        /// Teleport enemy
        /// </summary>
        public async void ResetMovePath()
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2002);
            if (towerStat != null)
            {
                var upgradeLevel = towerStat.towerLevel;
                var tower2002BonusStat =
                    GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(upgradeLevel);
                var backPos = LineController.GetBackwardPosition(transform.position, tower2002BonusStat.distanceUnit);

                if (upgradeLevel >= 3)
                {
                    multiMove.ResetMove();
                }
                else
                {
                    var vfx = ResourceUtils.GetVfxTower("2002_skill_2_impact_teleport_out", backPos,
                        Quaternion.identity);

                    UnitState.Set(ActionState.None);
                    gameObject.SetActive(false);
                    await UniTask.Delay(1000);
                    //multiMove.ResetMove();
                    //transform.position = backPos;
                    gameObject.SetActive(true);
                    var points = LineController.CalculateRemainPathWayPoints(backPos);
                    if (!points.Contains(backPos))
                    {
                        points.Insert(0, backPos);
                        MoveToEndPoint(points);
                    }
                    else
                    {
                        MoveToEndPoint(points);
                    }

                    //Debug.LogAssertion($"Teleport {Id} to position {backPos}");
                }
            }
            else
            {
                //null user data
                var tower2002BonusStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(1);
                var backPos = LineController.GetBackwardPosition(transform.position, tower2002BonusStat.distanceUnit);

                var vfx = ResourceUtils.GetVfxTower("2002_skill_2_impact_teleport_out", backPos, Quaternion.identity);
                LeanPool.Despawn(vfx, 0.5f);
                gameObject.SetActive(false);
                UnitState.Set(ActionState.None);
                await UniTask.Delay(1000);

                //multiMove.ResetMove();
                //transform.position = backPos;
                gameObject.SetActive(true);

                var points = LineController.CalculateRemainPathWayPoints(backPos);
                if (!points.Contains(backPos))
                {
                    points.Insert(0, backPos);
                    MoveToEndPoint(points);
                }
                else
                {
                    MoveToEndPoint(points);
                }

                //Debug.LogAssertion($"Tower stat null, teleport {Id} to position {backPos}");
            }

            multiMove.ResetMove();
        }

        public void ResetMove()
        {
            multiMove.ResetMove();
        }

        public override AttackMelee GetAttackMelee() => attackMelee;

        protected IEnumerator EffectDie()
        {
            var scaleCache = transform.localScale;

            var scale = transform.localScale;

            for (int i = 0; i < 2; i++)
            {
                scale.x = 0;
                transform.localScale = scale;
                yield return new WaitForSeconds(0.4f);
                transform.localScale = scaleCache;
                yield return new WaitForSeconds(0.4f);
            }

            transform.localScale = scale;

            blockCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            BodyCollider.gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);

            idle.Execute();

            yield return new WaitForEndOfFrame();

            LeanPool.Despawn(gameObject);
        }

        public override void SetColdOverrideMaterial()
        {
            var changer = GetComponent<SpineMaterialChanger>();
            if (changer)
                changer.SetCustomMaterialOverrides();
        }

        public override void RemoveColdOverrideMaterial()
        {
            var changer = GetComponent<SpineMaterialChanger>();
            if (changer)
                changer.RemoveCustomMaterialOverrides();
        }
#if UNITY_EDITOR
        [SerializeField] private float hp;
        [SerializeField] private float damage;
        [SerializeField] private float resistance;
        [SerializeField] private float armor;
        [SerializeField] private float critDamage;
        private void Update()
        {
            this.hp = Stats.GetStat(RPGStatType.Health).StatValue;
            this.damage = Stats.GetStat(RPGStatType.Damage).StatValue;
            this.resistance = Stats.GetStat(RPGStatType.Resistance).StatValue;
            this.armor = Stats.GetStat(RPGStatType.Armor).StatValue;
            this.critDamage =  Stats.GetStat(RPGStatType.CritDamage).StatValue;
        }
#endif
    }
}