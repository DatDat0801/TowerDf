using Invoke;
using UnityEngine;

namespace EW2
{
    public class Tower2003AtkRangeImpact : DamageBox<Tower2003>
    {
        private GetDamageCalculation damageCalculation;

        public void InitAOE(Tower2003 shooter)
        {
            owner = shooter;

            damageCalculation = new GetDamageCalculation(owner);

            Trigger(0.1f);
        }


        protected override bool CanGetDamage(Unit target)
        {
            return isAoe && target.IsAlive && ((EnemyBase) target).MoveType != MoveType.Fly;
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

                showVfxNormalAtk = true
            };

            (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);

            return damageInfo;
        }
    }
}