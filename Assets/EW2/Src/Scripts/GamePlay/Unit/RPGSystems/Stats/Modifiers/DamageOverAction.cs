using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class DamageOverAction : StatusOverTime
    {
        private readonly HealthPoint healthPoint;
        
        public DamageOverAction(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.DOTFollowTrigger;
            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            healthPoint.TakeDamage(Value, config.creator);
            
            Debug.Log($"Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
        }

        public override void Prepare()
        {
            // throw new System.NotImplementedException();
        }

        public override void Complete()
        {
            Debug.Log("Remove DamageOverTimeFollowAction: " + config.owner.name);

            config.owner.StatusController.RemoveStatus(this);
        }
    }
}