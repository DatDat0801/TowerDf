using UnityEngine;

namespace EW2
{
    public class Skill1Enemy3009DamageBox: EnemyNormalDamageBox
    {
        
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            var enemy3009 = (Enemy3009)owner;
            if (enemy3009.StatusController.CanUseSkill())
            {
                target.StatusController.AddStatus(enemy3009.skill1.CalculateStunStatus(target));
            }

            return base.GetDamage(target);
        }

       
    }
}