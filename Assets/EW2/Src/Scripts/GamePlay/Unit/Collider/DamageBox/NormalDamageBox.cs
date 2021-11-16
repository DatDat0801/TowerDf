using UnityEngine;

namespace EW2
{
    public abstract class NormalDamageBox<T> : DamageBox<T> where T : Unit
    {
        protected GetDamageCalculation damageCalculation;

        protected void Start()
        {
            damageCalculation = new GetDamageCalculation(owner);
        }
        
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                Debug.Log("Can't get damage");
                return null;
            }
            
            var damageInfo = new DamageInfo
            {
                creator = owner,
                
                damageType = owner.DamageType,
                
                showVfxNormalAtk = true,
                
                target = target
            };

            (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);
            return damageInfo;
        }
    }
}

