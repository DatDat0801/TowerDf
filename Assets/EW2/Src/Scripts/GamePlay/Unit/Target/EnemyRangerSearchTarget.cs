using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW2
{
    public class EnemyRangerSearchTarget : TargetCollection<Dummy>
    {
        protected override Dummy GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Enemy))
                return null;

            var unitCollider = other.GetComponent<BodyCollider>();

            if (unitCollider == null)
                throw new Exception("unit is null");
            if (unitCollider.Owner.UnitType == UnitType.Tower || unitCollider.Owner.UnitType == UnitType.None)
                return null;
            return unitCollider.Owner;
        }

        protected override void FilterTarget(Dummy target)
        {
            if (target == null || target.IsAlive == false)
            {
                Debug.Log("Target is null");
                return;
            }

            if (Targets.Contains(target))
            {
                // do nothing
                Debug.Log("Exist unit: " + target.Transform.name);
            }
            else
            {
                // Debug.Log("Add target: " + target.name);
                Targets.Add(target);
            }
        }

        public override Dummy SelectTarget()
        {
            if (Targets.Count > 0)
            {
                var targets = Targets.ToList();
                if (targets.Count > 0)
                {
                    foreach (var dummy in targets)
                    {
                        if (!dummy.IsAlive) continue;

                        return dummy;
                    }
                }
            }

            return null;
        }

        public override void RemoveTarget(Dummy target)
        {
            Targets.Remove(target);
        }
    }
}