using UnityEngine;

namespace EW2
{
    public class Skill1Enemy3015DamageBox: EnemyNormalDamageBox
    {
        
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            var enemy3015 = (Enemy3015)owner;
            if (enemy3015.StatusController.CanUseSkill())
            {
                target.StatusController.AddStatus(enemy3015.skill1.CalculateStunStatus(target));
            }

            return base.GetDamage(target);
        }
    }
}