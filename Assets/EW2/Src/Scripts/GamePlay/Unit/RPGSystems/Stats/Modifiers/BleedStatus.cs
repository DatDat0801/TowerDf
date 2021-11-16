using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class BleedStatus : StatusOverTime
    {
        private Action onAttackStart;

        public BleedStatus(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Bleed;
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
        }

        public override void Prepare()
        {
            if (config != null)
            {
                var attackMelee = ((Dummy)config.owner).GetAttackMelee();
                if (attackMelee != null)
                {
                    attackMelee.onStart = ApplyBleed;
                }
            }

            // do nothing
        }

        public override void Complete()
        {
            // do nothing. Never complete
            if (config != null)
            {
                var attackMelee = ((Dummy)config.owner).GetAttackMelee();
                if (attackMelee != null)
                {
                    attackMelee.onStart = null;
                }
            }

        }

        private void ApplyBleed()
        {
            var damageInfo = new DamageInfo() {
                creator = config.creator,
                attackType = AttackType.DOT,
                damageType = DamageType.Physical,
                isCritical = false,
                value = Value
            };
            ((Dummy)config.owner).GetHurt(damageInfo);
            // Debug.Log($"Bleed Trigger {config.owner.name} |  | Damage {damageInfo.value}");
        }
    }
}