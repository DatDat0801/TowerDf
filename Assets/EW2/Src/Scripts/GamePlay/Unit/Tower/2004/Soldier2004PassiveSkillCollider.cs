using UnityEngine;

namespace EW2
{
    public class Soldier2004PassiveSkillCollider : DamageBox<Soldier2004>
    {
        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var enemy = (EnemyBase) target;

            if (enemy.MoveType != owner.SoldierData.searchTarget)
            {
                return null;
            }

            print($"{owner.name} passive 1");

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = owner.DataSkillCounter.damage
            };


            return damageInfo;
        }
    }
}