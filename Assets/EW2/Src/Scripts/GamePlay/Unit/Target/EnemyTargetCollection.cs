using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW2
{
    public class EnemyTargetCollection : TargetCollection<Dummy>
    {
        protected override Dummy GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Hero) == false)
                return null;

            var unitCollider = other.GetComponentInParent<Dummy>();

            // if (unitCollider == null)
            //     throw new Exception("unit is null");

            return unitCollider;
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
            return default;
        }

        public override void RemoveTarget(Dummy target)
        {
            Targets.Remove(target);
        }

        public List<Dummy> CalculateAllTarget() => Targets.ToList().FilterByType(UnitType.Hero);
    }
}
