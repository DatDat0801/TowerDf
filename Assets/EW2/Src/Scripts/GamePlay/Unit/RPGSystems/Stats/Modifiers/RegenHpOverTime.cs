using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class RegenHpOverTime : StatusOverTime
    {
        protected HealthPoint healthPoint;

        public RegenHpOverTime(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.HealInstantOverTime;
            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);

        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            healthPoint.CurrentValue += Value;
            callback?.Invoke();
            
            if (healthPoint.IsFull)
            {
                Stop();
            }

            Debug.Log($"Added HP {Value}, Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
        }

        public override void Prepare()
        {
            // do nothing
        }

        public override void Complete()
        {
            // do nothing. Never complete
        }
    }
}