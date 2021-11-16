using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class AllyBlockEnemy : AllyTargetCollection
    {
        private int blockNumber;

        public int BlockNumber
        {
            get => blockNumber;
            set => blockNumber = value;
        }

        private Dummy owner;
        private CancellationTokenSource cts;
        private Coroutine _coroutine;
        private bool IsEnable => gameObject.activeSelf;

        private ObservableList<EnemyBase> blockList;
        public ObservableList<EnemyBase> BlockList => blockList ?? (blockList = new ObservableList<EnemyBase>());

        protected void Start()
        {
            InitTriggerTargetChange();
        }

        private void InitTriggerTargetChange()
        {
            Targets.CollectionChanged += (sender, args) => {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var target in args.NewItems)
                        {
                            SetBlock(target as EnemyBase);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var target in args.OldItems)
                        {
                            SetUnblock(target as EnemyBase);
                            //SetBlock(null);
                        }

                        break;
                    default:
                        throw new Exception("Do not support this notify");
                }
            };
        }

        public void SetOwner(AllySearchTarget targetController)
        {
            this.owner = targetController.Owner;
        }

        private readonly List<EnemyBase> tempTargets = new List<EnemyBase>();

        private void SetBlock(EnemyBase target)
        {
            if (target == null)
            {
                UpdateTargets();
                return;
            }

            if (target.MoveType == MoveType.Fly) return;

            if (!tempTargets.Contains(target))
            {
                tempTargets.Add(target);
            }

            //Debug.Log("Enemy: " + target.name);
            UpdateTargets();
        }

        void UpdateTargets()
        {
            if (tempTargets.Count >= 1)
            {
                if (this._coroutine != null)
                {
                    CoroutineUtils.Instance.StopCoroutine(this._coroutine);
                    this._coroutine = null;
                }
                
                this._coroutine = CoroutineUtils.Instance.StartCoroutine(WaitToUpdateTarget());
                // cts?.Cancel();
                // if (cts == null || cts.IsCancellationRequested)
                //     cts = new CancellationTokenSource();
                //
                // UniTask.Run(async () =>
                // {
                //     await UniTask.SwitchToMainThread();
                //     await UniTask.DelayFrame(1, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

                // tempTargets.NearestTarget(owner);
                //
                //
                // //UniTask.WaitUntil(() => {
                // foreach (var tempTarget in tempTargets)
                // {
                //     if (BlockList.Count >= BlockNumber)
                //         break;
                //
                //     if (this.owner != null && CanBlock(tempTarget))
                //     {
                //         tempTarget.blockCollider.SetBlock(owner);
                //         BlockList.Add(tempTarget);
                //     }
                // }

                //if (BlockList.Count >= BlockNumber || BlockList.Count <= 0)
                //{
                //return true;
                //}

                //return false;
                //});
                //}).WithCancellation(cts.Token);
            }
        }

        IEnumerator WaitToUpdateTarget()
        {
            yield return new WaitForSeconds(0.02f);
            tempTargets.NearestTarget(owner);

            foreach (var tempTarget in tempTargets)
            {
                if (BlockList.Count >= BlockNumber)
                    break;

                if (this.owner != null && CanBlock(tempTarget))
                {
                    tempTarget.blockCollider.SetBlock(owner);
                    BlockList.Add(tempTarget);
                }
            }
        }

        private void SetUnblock(EnemyBase target)
        {
            if (owner != null && IsContain(target))
            {
                //Debug.Log($"Unblock: {target.name} => {owner.name}");

                target.blockCollider.RemoveBlock();

                RemoveBlock(target);

                if (tempTargets.Contains(target))
                {
                    tempTargets.Remove(target);
                }

                if (IsEnable)
                {
                    RecheckBlock();
                }
            }
            else if (tempTargets.Contains(target))
            {
                tempTargets.Remove(target);
            }
        }

        private void RecheckBlock()
        {
            foreach (var target in Targets)
            {
                var enemy = target;
                if (CanBlock(enemy))
                {
                    // Debug.LogWarning("Reblock");
                    //SetBlock(enemy);
                    UpdateTargets();
                    break;
                }
            }
        }

        public void RemoveBlock(EnemyBase enemy)
        {
            var result = BlockList.Remove(enemy);
            if (result == false)
            {
                Debug.Log("Enemy not exist: " + enemy.name);
            }
        }

        private bool IsContain(EnemyBase enemy)
        {
            return BlockList.Contains(enemy);
        }

        public bool CanBlock(EnemyBase enemy)
        {
            return BlockList.Contains(enemy) == false &&
                   enemy.blockCollider.CanTakeBlock(owner);
        }
    }
}