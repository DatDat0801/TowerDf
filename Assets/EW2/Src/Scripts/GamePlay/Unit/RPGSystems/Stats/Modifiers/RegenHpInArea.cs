using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class RegenHpInArea : RegenHpOverTime
    {
        public RegenHpInArea(StatusOverTimeConfig config) : base(config)
        {
            
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            if (!healthPoint.IsFull)
            {
                healthPoint.CurrentValue += Value;
                callback?.Invoke();
            }

            Debug.Log($"Current HP on regen in a Area: {healthPoint.CurrentValue}/{healthPoint.StatValue}");
        }
    }
}