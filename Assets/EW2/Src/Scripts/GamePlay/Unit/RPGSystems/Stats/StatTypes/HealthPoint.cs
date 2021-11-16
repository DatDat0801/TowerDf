using UnityEngine;

namespace EW2
{
    public class HealthPoint : RPGAttribute
    {
        
        /// <summary>
        ///  current HP of object
        /// </summary>
        private float currentValue;

        private ShieldPoint shieldPoint;
        
        public float CurrentValue
        {
            get => Mathf.Clamp(currentValue, 0, StatValue);
            set => Set(ref currentValue, Mathf.Clamp(value, 0, StatValue));
        }

        public bool IsFull => CurrentValue >= StatValue;

        public float CalculateCurrentPercent() => currentValue / StatValue;

        public void AddShieldPoint(ShieldPoint shieldPoint)
        {
            shieldPoint?.Remove();

            this.shieldPoint = shieldPoint;
            
            this.shieldPoint.Execute();
        }

        public void RemoveShieldPoint()
        {
            shieldPoint = null;
        }

        public void TakeDamage(float value, Unit creator)
        {
            shieldPoint?.TakeDamage(ref value, creator);

            if (value > 0)
            {
                CurrentValue -= value;
            }
        }
        

        public override void ClearModifiers()
        {
            shieldPoint?.Remove();
            
            base.ClearModifiers();
        }
        
    }
}