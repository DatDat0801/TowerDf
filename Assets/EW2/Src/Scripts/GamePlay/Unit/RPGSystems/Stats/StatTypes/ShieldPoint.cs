using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class ShieldPoint : StatusOverTime
    {
        private readonly HealthPoint healthPoint;

        public ShieldPoint(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.ShieldPoint;

            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);

            Debug.Log($"New Shield Point: Name[{config.owner.name}], Time[{config.lifeTime}], HP[{config.baseValue}]");
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            // do something
        }

        public override void Prepare()
        {
            // do nothing
        }

        public override void Complete()
        {
        }

        public override void Remove()
        {
            try
            {
                if (config.owner != null)
                    Debug.Log("Remove Shield: " + config.owner.name);

                Stop();

                healthPoint?.RemoveShieldPoint();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void TakeDamage(ref float value, Unit owner)
        {
            if (this.Value - value <= 0.001f)
            {
                value -= this.Value;

                this.Value = 0;
            }
            else
            {
                this.Value -= value;

                value = 0;
            }

            if (this.Value <= 0)
            {
                Remove();
            }

            Debug.Log("Current ShieldPoint: " + this.Value);
        }
    }
}