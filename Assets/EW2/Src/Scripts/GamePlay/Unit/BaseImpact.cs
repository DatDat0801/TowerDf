using UnityEngine;

namespace EW2
{
    public class ImpactConfig
    {
        public float damage;

        public Unit owner;

        public Unit target;
    }

    public class BaseImpact : MonoBehaviour, IGetDamage
    {
        protected ImpactConfig config;

        protected GetDamageCalculation damageCalculation;

        protected DamageInfo damageInfo;

        protected void InitDamageInfo(Unit target)
        {
            damageInfo = new DamageInfo
            {
                creator = config.owner,
                damageType = config.owner.DamageType, target = target
            };

            if (config.damage <= 0f)
            {
                damageCalculation = new GetDamageCalculation(config.owner);

                (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(config.target);
            }
            else
            {
                damageInfo.value = config.damage;
            }
        }

        public virtual DamageInfo GetDamage(Unit target)
        {
            if (config.target != null && config.target != target) return null;

            InitDamageInfo(target);

            return damageInfo;
        }
    }
}