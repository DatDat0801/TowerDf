using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class DamageOverTime : StatusOverTime
    {
        private Dummy dummy;

        public DamageOverTime(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.DOT;

            dummy = (Dummy) config.owner;
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
                attackType = AttackType.DOT,
                damageType = DamageType.True,
                isCritical = false,
                value = Value
            };
            dummy.GetHurt(damageInfo);
            
            // var healthPoint = config.owner.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            //
            // Debug.Log($"Current HP: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
        }

        public override void Prepare()
        {
        }

        public override void Complete()
        {
            Debug.Log("Remove DOT: " + config.owner.name);

            config.owner.StatusController.RemoveStatus(this);
        }
    }
}