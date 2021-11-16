using UnityEngine;

namespace EW2
{
    public class Skill1Enemy3017DamageBox : EnemyNormalDamageBox
    {
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            var enemy3017 = (Enemy3017) owner;

            target.StatusController.AddStatus(enemy3017.skill1.CalculateStunStatus(target));

            return base.GetDamage(target);
        }

        protected override void OnEnable()
        {
            Trigger();
        }
    }
}