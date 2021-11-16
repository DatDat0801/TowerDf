using System;
using UnityEngine;

namespace EW2
{
    public class Hero1004Passive3Collider : TargetCollection<HeroBase>
    {
        protected override HeroBase GetTarget(Collider2D other)
        {
            var unitCollider = other.GetComponent<BodyCollider>();

            if (unitCollider == null)
                throw new Exception("unit is null");

            if (!(unitCollider.Owner is HeroBase)) return null;
            
            return unitCollider.Owner as HeroBase;
        }

        protected override void FilterTarget(HeroBase target)
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
                Targets.Add(target);
            }
        }

        public override void RemoveTarget(HeroBase target)
        {
            Targets.Remove(target);
        }

        public override HeroBase SelectTarget()
        {
            return null;
        }
    }
}