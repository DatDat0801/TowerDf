using UnityEngine;

namespace EW2
{
    public class Hero1003PassiveSkill2Collider : DamageBox<Hero1003>
    {
        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var enemy = (EnemyBase) target;

            if (owner.HeroStatBase.searchTarget != MoveType.All && enemy.MoveType != owner.HeroStatBase.searchTarget)
            {
                return null;
            }
            
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = owner.passiveSkill2.GetDamage(), target = enemy
            };

            //Debug.LogAssertion("Get damage value skill active 2: " + damageInfo.value);
            return damageInfo;
        }
    }
}