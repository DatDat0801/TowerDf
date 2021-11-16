using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class AllySearchTarget : MonoBehaviour
    {
        public Dummy Owner;

        public readonly ObservableProperty<EnemyBase> target = new ObservableProperty<EnemyBase>(null);

        public readonly ObservableProperty<AttackType> targetAttackType =
            new ObservableProperty<AttackType>(AttackType.None);

        [SerializeField] protected AllyBlockEnemy allyBlockEnemy;

        private Coroutine coroutineWaitUpdateTargetList;

        private MoveType targetType;

        public bool IsEnable => gameObject.activeSelf;

        public bool HasTarget => target.Value != null;

        protected virtual void Start()
        {
            allyBlockEnemy.SetOwner(this);
            allyBlockEnemy.BlockList.CollectionChanged += UpdateBlockList;
            allyBlockEnemy.Targets.CollectionChanged += UpdateTargetList;
        }

        private void OnDestroy()
        {
            allyBlockEnemy.BlockList.CollectionChanged -= UpdateBlockList;
            allyBlockEnemy.Targets.CollectionChanged -= UpdateTargetList;
        }

        private void OnDisable()
        {
            StopUpdateList();
        }

        private void UpdateTargetList(object sender, NotifyCollectionChangedEventArgs args)
        {
            StopUpdateList();

            coroutineWaitUpdateTargetList = CoroutineUtils.Instance.StartCoroutine(IUpdateTargetList());
        }

        private void StopUpdateList()
        {
            if (coroutineWaitUpdateTargetList != null)
            {
                CoroutineUtils.Instance.StopCoroutine(coroutineWaitUpdateTargetList);
                coroutineWaitUpdateTargetList = null;
            }
        }

        private IEnumerator IUpdateTargetList()
        {
            yield return new WaitForSeconds(0.035f);

            if (allyBlockEnemy.BlockList.Count > 0) yield break;

            if (targetAttackType.Value != AttackType.Range)
                (target.Value, targetAttackType.Value) = SelectTarget();

            coroutineWaitUpdateTargetList = null;
        }

        private void UpdateBlockList(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var target in args.NewItems)
                    {
                        UpdateBlockTarget(target as EnemyBase, args.Action);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var target in args.OldItems)
                    {
                        UpdateBlockTarget(target as EnemyBase, args.Action);
                    }

                    break;
                default:
                    throw new Exception("Do not support this notify");
            }
        }

        public virtual void SetTargetType(MoveType moveType)
        {
            this.targetType = moveType;

            allyBlockEnemy.SetTargetType(targetType);
        }

        public void SetBlockNumber(int blockNumber)
        {
            allyBlockEnemy.BlockNumber = blockNumber;
        }

        public void UpdateBlockTarget(EnemyBase enemy, NotifyCollectionChangedAction action)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAddEnemyToBlockList(enemy);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnRemoveEnemyFromBlockList(enemy);
                    break;
            }
        }

        protected virtual void OnAddEnemyToBlockList(EnemyBase enemy)
        {
            //Debug.LogWarning(Owner.name + "====> Event Add To Block");

            if (target.Value == null || !allyBlockEnemy.BlockList.Contains(target.Value) || !target.Value.IsAlive)
            {
                //Debug.LogWarning(Owner.name + $"====> Set Enemy {enemy.UnitId}");
                (target.Value, targetAttackType.Value) = SelectTarget();
            }
            else if (target.Value != null)
            {
                (target.Value, targetAttackType.Value) = (target.Value, AttackType.Melee);
            }
        }

        protected virtual void OnRemoveEnemyFromBlockList(EnemyBase enemy)
        {
            // Debug.LogWarning("Event Remove From Block");

            if (target.Value != null && target.Value.Id == enemy.Id)
            {
                //Debug.LogWarning(Owner.name + $"====> Remove Enemy {enemy.UnitId} From Block");
                (target.Value, targetAttackType.Value) = (null, AttackType.None);
            }

            if (IsEnable && target.Value == null)
            {
                (target.Value, targetAttackType.Value) = SelectTarget();
            }
        }

        protected virtual (EnemyBase, AttackType) SelectTarget()
        {
            EnemyBase enemy;


            if (allyBlockEnemy.BlockList.Count > 0)
            {
                enemy = allyBlockEnemy.BlockList[0];
            }
            else
            {
                enemy = allyBlockEnemy.SelectTarget();
            }

            if (enemy == null || !enemy.CanChooseIsTarget)
                return (null, AttackType.None);

            return (enemy, AttackType.Melee);
        }

        public float Distance()
        {
            if (target.Value)
            {
                var posMelee = MathUtils.GetPositionToMeleeAttack(transform.position, target.Value.Transform.position, 0.3f);
                
                return Vector2.Distance(Owner.transform.position, posMelee);
            }

            //Debug.Log("Target is null");
            return 0;
        }

        public void SetRangeBlock(float radius)
        {
            var circleCollider = allyBlockEnemy.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.radius = radius;
            }
        }
        
        public List<EnemyBase> SelectTargets(int quantity)
        {
            var targets = allyBlockEnemy.Targets.ToList().FilterByType(UnitType.Enemy).Take(quantity);
            return targets.ToList();
        }
    }
}