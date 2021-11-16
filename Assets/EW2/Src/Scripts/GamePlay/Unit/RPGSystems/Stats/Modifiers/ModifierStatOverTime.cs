using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class ModifierStatOverTime : StatusOverTime
    {
        public Action OnCompleted { get; set; }

        private RPGStatModifier rpgStatModifier;


        public ModifierStatOverTime(StatusOverTimeConfig config, RPGStatModifier rpgStatModifier) : base(config)
        {
            statusType = config.statusType;

            this.rpgStatModifier = rpgStatModifier;
        }

        public override void UpdateValue()
        {
            Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            rpgStatModifier.Stat.AddModifier(rpgStatModifier);
        }

        public override void Prepare()
        {
            // do nothing
        }

        public override void Complete()
        {
            rpgStatModifier?.Stat.RemoveModifier(rpgStatModifier);
            OnCompleted?.Invoke();
        }
    }
}