using System;
using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Tower2001BuffAtkSpeed : TargetCollection<Building>
    {
        public Collider2D collider2D;
        private Tower2001 owner;
        public Tower2001 Owner => owner;

        private TowerData2001.Skill2 dataSkill2;

        public void ActiveSkill(Tower2001 tower2001, TowerData2001.Skill2 dataSkill)
        {
            gameObject.SetActive(false);
            this.owner = tower2001;
            this.dataSkill2 = dataSkill;
            collider2D.enabled = true;
            gameObject.SetActive(true);
        }

        public void DeactiveSkill()
        {
            collider2D.enabled = false;
            if (gameObject != null && InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.Invoke(this, () => { LeanPool.Despawn(this); }, 0.2f);
        }

        private void AddBuff(Building target, bool isOwner)
        {
            if (target != null)
            {
                RPGStatModifier atkSpeeStatModifier = new RPGStatModifier(new AttackSpeed(), dataSkill2.modifierType,
                    dataSkill2.atkSpeedBonusPercent, false,
                    this.owner, target);
                target.AddBuffAtkSpeed(this, atkSpeeStatModifier, isOwner);
            }
        }

        private void RemoveBuff(Building target)
        {
            if (target != null)
            {
                target.DebuffAtkSpeed(this);
            }
        }

        protected override Building GetTarget(Collider2D other)
        {
            if (other.CompareTag(TagName.Tower) == false)
                return null;

            var unitCollider = other.GetComponent<Building>();

            if (unitCollider == null)
                throw new Exception("unit is null");

            return unitCollider;
        }

        protected override void FilterTarget(Building target)
        {
            if (target == null)
            {
                return;
            }

            if (Targets.Contains(target))
            {
                // do nothing
                //Debug.Log("Exist unit: " + target.Transform.name);
            }
            else
            {
                // Debug.Log("Add target: " + target.name);
                Targets.Add(target);
                AddBuff(target, this.owner.gameObject.GetInstanceID() == target.gameObject.GetInstanceID());
            }
        }

        public override Building SelectTarget()
        {
            return null;
        }

        public override void RemoveTarget(Building target)
        {
            Targets.Remove(target);
            RemoveBuff(target);
        }
    }
}