using UnityEngine;

namespace EW2
{
    public class Skill1Enemy3011DamageBox: EnemyNormalDamageBox
    {
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            var enemy3011 = (Enemy3011)owner;
            if (enemy3011.StatusController.CanUseSkill())
            {
                target.StatusController.AddStatus(enemy3011.Skill1.CalculateBleedStatus(target));
            }

            return base.GetDamage(target);
        }
    }
}