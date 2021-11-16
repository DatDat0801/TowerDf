using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW2
{
    public class AllyTargetCollection : TargetCollection<EnemyBase>
    {
        protected override EnemyBase GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Enemy) == false)
                return null;

            var unitCollider = other.GetComponent<EnemyBlockCollider>();

            if (unitCollider == null)
                throw new Exception("unit is null");

            return unitCollider.Owner;
        }

        protected override void FilterTarget(EnemyBase target)
        {
            if (target == null || target.IsAlive == false)
            {
                Debug.Log("Target is null");
                return;
            }

            if (targetType != MoveType.All)
            {
                if (targetType != target.EnemyData.moveType)
                {
                    return;
                }
            }

            if (Targets.Contains(target))
            {
                // do nothing
                Debug.Log("Exist unit: " + target.Transform.name);
            }
            else
            {
                Targets.Add(target);
            }
        }

        public override void RemoveTarget(EnemyBase target)
        {
            Targets.Remove(target);
        }

        public override EnemyBase SelectTarget()
        {
            if (Targets.Count > 0)
            {
                var targets = Targets.ToList().FilterByType(UnitType.Enemy);
                if (targets.Count > 0)
                {
                    foreach (var enemy in targets)
                    {
                        if (enemy.MoveType == MoveType.Fly || !enemy.IsAlive) continue;

                        return enemy;
                    }
                }
            }

            return null;
        }

        public List<EnemyBase> CalculateAllTarget() => Targets.ToList().FilterByType(UnitType.Enemy);
    }
}