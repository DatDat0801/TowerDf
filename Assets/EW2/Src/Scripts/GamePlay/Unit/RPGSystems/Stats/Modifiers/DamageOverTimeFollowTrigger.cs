using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class DamageOverTimeFollowTrigger : StatusOverTime
    {
        private readonly HealthPoint healthPoint;

        public delegate bool Trigger();

        public event Trigger triggerTakeDamage;

        public DamageOverTimeFollowTrigger(StatusOverTimeConfig config, Trigger trigger) :
            base(config)
        {
            statusType = StatusType.DOTFollowTrigger;
            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            this.triggerTakeDamage = trigger;
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            if (triggerTakeDamage != null && triggerTakeDamage.Invoke())
            {
                healthPoint.TakeDamage(Value, config.creator);
                Debug.Log($"Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
            }
        }

        public override void Prepare()
        {
        }

        public override void Complete()
        {
            Debug.Log("Remove DamageOverTimeFollowAction: " + config.owner.name);

            config.owner.StatusController.RemoveStatus(this);
        }
    }
}