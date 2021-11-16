using System.Collections.Specialized;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class EnemyRanger : EnemyBase
    {
        public EnemyRangerSearchTarget rangeSearchTarget;

        private float rangeAttackRange;
        private CircleCollider2D circleCollider;

        protected override void Awake()
        {
            base.Awake();

            rangeSearchTarget.Targets.CollectionChanged += UpdateTargetList;

            circleCollider = rangeSearchTarget.GetComponent<CircleCollider2D>();
        }

        private void UpdateTargetList(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (var target in args.OldItems)
                    {
                        var targetRemove = target as Dummy;
                        if (this.target == targetRemove)
                        {
                            this.target = null;
                            break;
                        }
                    }

                    break;
            }
        }

        protected override bool IsInRangeAttackRange()
        {
            if (!blockCollider.CanTakeBlock(null) || UnitState.Current == ActionState.Die) return false;

            target = rangeSearchTarget.SelectTarget();


            if (target != null)
            {
                if (target.UnitType == UnitType.Tower) return false;
                if (DistanceToTarget() <= rangeAttackRange)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        protected override void InitCollider(float timeDelay)
        {
            rangeSearchTarget.gameObject.SetActive(false);
            base.InitCollider(timeDelay);
            InvokeProxy.Iinvoke.Invoke(this, () => { rangeSearchTarget.gameObject.SetActive(true); }, 3f);
        }

        public override void EnableColliderImmediate()
        {
            base.EnableColliderImmediate();
            rangeSearchTarget.gameObject.SetActive(true);
        }

        public override void SetInfo(int id, int level)
        {
            base.SetInfo(id, level);
            if (circleCollider != null)
            {
                rangeAttackRange = EnemyData.detectRangeAttack;
                circleCollider.radius = EnemyData.detectRangeAttack;
            }
        }
    }
}