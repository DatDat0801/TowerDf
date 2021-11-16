using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class RangerSearchTarget : AllySearchTarget
    {
        [SerializeField] protected AllyTargetCollection rangerCollider;

        private List<EnemyBase> listRanger = new List<EnemyBase>();

        private CancellationTokenSource cts;

        protected override void Start()
        {
            base.Start();

            InitTriggerRangerListChange();

            // if (allyBlockEnemy)
            //     allyBlockEnemy.BlockList.CollectionChanged += UpdateBlockList;
        }

        // private void UpdateBlockList(object sender, NotifyCollectionChangedEventArgs args)
        // {
        //     switch (args.Action)
        //     {
        //         case NotifyCollectionChangedAction.Remove:
        //             if (IsEnable)
        //                 ReCheckTarget();
        //             break;
        //     }
        // }

        protected void InitTriggerRangerListChange()
        {
            rangerCollider.Targets.CollectionChanged += (sender, args) => {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var target in args.NewItems)
                        {
                            UpdateRangerTarget(target as EnemyBase, args.Action);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var target in args.OldItems)
                        {
                            UpdateRangerTarget(target as EnemyBase, args.Action);
                        }

                        break;
                    default:
                        throw new Exception("Do not support this notify");
                }
            };
        }

        public override void SetTargetType(MoveType moveType)
        {
            base.SetTargetType(moveType);

            rangerCollider.SetTargetType(moveType);
        }

        private void UpdateRangerTarget(EnemyBase enemy, NotifyCollectionChangedAction action)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAddEnemyToRangerList(enemy);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnRemoveEnemyFromRangerList(enemy);
                    break;
            }
        }

        protected void OnAddEnemyToRangerList(EnemyBase enemy)
        {
            if (!listRanger.Contains(enemy))
            {
                //Debug.LogWarning("****Add Enemy To Targets****" + enemy.name);
                cts?.Cancel();
                listRanger.Add(enemy);
            }

            if (listRanger.Count >= 1)
            {
                if (cts == null || cts.IsCancellationRequested)
                    cts = new CancellationTokenSource();

                UniTask.Run(async () => {
                    await UniTask.SwitchToMainThread();
                    await UniTask.DelayFrame(3, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

                    listRanger.NearestTarget(this.Owner);

                    if (target.Value == null && listRanger.Count > 0)
                        (target.Value, targetAttackType.Value) = SelectTargetRange();
                }).WithCancellation(cts.Token);
            }
        }

        protected void OnRemoveEnemyFromRangerList(EnemyBase enemy)
        {
            if (target.Value != null && target.Value.Equals(enemy))
            {
                //Debug.LogWarning("****Remove Enemy From Targets****" + enemy.name);
                (target.Value, targetAttackType.Value) = (null, AttackType.None);
            }

            listRanger.Remove(enemy);

            if (IsEnable && target.Value == null && listRanger.Count > 0)
            {
                (target.Value, targetAttackType.Value) = SelectTargetRange();
            }
        }

        protected (EnemyBase, AttackType) SelectTargetRange()
        {
            foreach (var tempTarget in listRanger)
            {
                if (!allyBlockEnemy.BlockList.Contains(tempTarget) && tempTarget.CanChooseIsTarget)
                    return (tempTarget, AttackType.Range);
            }

            return (null, AttackType.None);
        }

        protected override void OnRemoveEnemyFromBlockList(EnemyBase enemy)
        {
            base.OnRemoveEnemyFromBlockList(enemy);

            if (IsEnable && target.Value == null)
            {
                ReCheckTarget();
            }
        }

        private void ReCheckTarget()
        {
            listRanger.NearestTarget(this.Owner);
            if (target.Value == null && listRanger.Count > 0)
                (target.Value, targetAttackType.Value) = SelectTargetRange();
        }

        public void ScaleRangeDetect(float ratio)
        {
            rangerCollider.gameObject.SetActive(false);
            rangerCollider.transform.localScale = rangerCollider.transform.localScale * ratio;
            rangerCollider.gameObject.SetActive(true);
            (target.Value, targetAttackType.Value) = SelectTargetRange();
            EventManager.EmitEventData(GamePlayEvent.OnUpdateRangeAttack, rangerCollider.transform.localScale.x);
        }

        public void SetRangeAttack(float radius)
        {
            rangerCollider.transform.localScale = Vector3.one * radius / GameConfig.RatioConvertSizeRangeDetect;
        }

        public float GetRangeAttack()
        {
            var radius = GameConfig.RatioConvertSizeRangeDetect * rangerCollider.transform.localScale.x;
            return radius;
        }
        
        public List<EnemyBase> SelectTargets(int quantity)
        {
            var targets = rangerCollider.Targets.ToList().FilterByType(UnitType.Enemy).Take(quantity);
            return targets.ToList();
        }
        
    }
}