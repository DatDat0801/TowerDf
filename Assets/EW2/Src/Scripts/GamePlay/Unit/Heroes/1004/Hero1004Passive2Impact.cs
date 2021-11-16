using UnityEngine;

namespace EW2
{
    public class Hero1004Passive2Impact : TargetCollection<Dummy>
    {
        private Human1004 owner;
        private float timeLifeStatus;

        public void InitImpact(Human1004 hero1004, HeroData1004.PassiveSkill2 skillData)
        {
            owner = hero1004;
            timeLifeStatus = skillData.timeLife;
        }

        protected override Dummy GetTarget(Collider2D other)
        {
            if (!other.CompareTag(TagName.Enemy))
                return null;

            var unitCollider = other.GetComponentInParent<Dummy>();

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
                var silent = new SilentStatus(new StatusOverTimeConfig()
                {
                    creator = this.owner,
                    owner = target,
                    lifeTime = timeLifeStatus,
                    statusType = StatusType.Silent
                });

                target.StatusController.AddStatus(silent);
                
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
    }
}