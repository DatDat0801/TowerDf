using Invoke;
using UnityEngine;

namespace EW2.Spell
{
    public class Spell4003PassiveDamageImpact : SpellDamageImpactBase
    {
        private Spell4003PassiveData passiveData;

        public void Initialize(Spell4003PassiveData data, Unit spell)
        {

            owner = spell;
            passiveData = data;
            var circleCollider = (CircleCollider2D) damageBoxAOE;
            circleCollider.radius = passiveData.radius;

            Trigger(0.25f, 0.2f);
            InvokeProxy.Iinvoke.Invoke(this, RemoveFx, 4f);
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = passiveData.damage
            };

            return damageInfo;
        }
    }
}