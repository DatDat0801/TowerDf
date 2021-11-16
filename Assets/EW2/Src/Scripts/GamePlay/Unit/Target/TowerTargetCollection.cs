using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EW2
{
    public class TowerTargetCollection : TargetCollection<Building>
    {
        public virtual float Radius { get; set; }

        protected override Building GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Tower) == false)
                return null;
            var target = other.GetComponent<Building>();
            Assert.IsNotNull(target);
            return target;
        }

        protected override void FilterTarget(Building target)
        {
            if (target == null || !target.IsAlive)
            {
                return;
            }

            if (!Targets.Contains(target))
            {
                Targets.Add(target);
            }
        }

        /// <summary>
        /// Get a quantity of building that is not stunning
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public virtual List<Building> SelectTarget(int quantity)
        {
            var results = new List<Building>();
            for (var i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].UnitState.Current != ActionState.Stun && results.Count < quantity)
                {
                    results.Add(Targets[i]);
                }
            }

            return results;
        }

        public override Building SelectTarget()
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveTarget(Building target)
        {
            Targets.Remove(target);
        }
    }
}