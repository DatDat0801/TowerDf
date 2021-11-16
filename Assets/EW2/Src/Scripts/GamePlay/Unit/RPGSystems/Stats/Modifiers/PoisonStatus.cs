using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class PoisonStatus : StatusOverTime
    {
        private readonly HealthPoint healthPoint;
        
        public PoisonStatus(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Poison;
            healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            var damageInfo = new DamageInfo()
            {
                creator =  config.creator,
                attackType = AttackType.None,
                damageType = config.damageType,
                isCritical = false,
                value = Value
            };
                
            ((Dummy)config.owner).GetHurt(damageInfo);

            // do not for all, just basic and demo
            //healthPoint.CurrentValue -= Value;

            if (healthPoint.CurrentValue <= 0)
            {
                Stop();
                if (config.owner.IsAlive)
                {
                    config.owner.Remove();
                }
            }

            Debug.Log($"Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
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