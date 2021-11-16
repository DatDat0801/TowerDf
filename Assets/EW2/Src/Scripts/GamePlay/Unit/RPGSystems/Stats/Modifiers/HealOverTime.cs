using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class HealOverTime : StatusOverTime
    {
        private readonly HealthPoint healthPoint;
        
        public HealOverTime(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.HealInstantOverTime;
            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);
        }

        public override void UpdateValue()
        {
            // heal instant 

            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            // do not for all, just basic and demo
            healthPoint.CurrentValue += Value;

            Debug.Log($"Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
        }

        public override void Prepare()
        {
            // do nothing
        }

        public override void Complete()
        {
            Debug.Log("Remove Heal: " + config.owner.name);

            config.owner.StatusController.RemoveStatus(this);
        }
    }
}