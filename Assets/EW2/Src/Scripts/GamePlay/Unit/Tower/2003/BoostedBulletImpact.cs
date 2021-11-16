using UnityEngine;

namespace EW2
{
    public class BoostedBulletImpact: DamageBox<Tower2003>
    {
        private GetDamageCalculation damageCalculation;

        public void InitAOE(Tower2003 shooter)
        {
            owner = shooter;

            damageCalculation = new GetDamageCalculation(owner);

            Trigger(0.01f);
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

            //var tower2003 = (Tower2003) owner;
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                showVfxNormalAtk = true,
                
                value = owner.towerData.BonusStat.level3Stat.fireBulletDamage,
                
                isCritical = false
            };
            
            //(damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);

            return damageInfo;
        }
    }
}