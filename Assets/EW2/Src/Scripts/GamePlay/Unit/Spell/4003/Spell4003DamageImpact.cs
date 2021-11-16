using Invoke;
using UnityEngine;

namespace EW2.Spell
{
    public class Spell4003DamageImpact : SpellDamageImpactBase
    {
        //private SpellStatBase spellStat;
        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = owner, 
                
                damageType = owner.DamageType,
                
                value =  this.owner.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue
            };

            return damageInfo;
        }
        public void Initialize(SpellStatBase statBase, Unit spell)
        {
            owner = spell;
            //spellStat = statBase;
            //var circleCollider = (CircleCollider2D) damageBoxAOE;
            //circleCollider.radius = statBase.range;
            transform.localScale = Vector3.one * statBase.range;
            
            Trigger(0.25f, 0.55f);
            InvokeProxy.Iinvoke.Invoke(this, RemoveFx, 4f);
        }
        
    }
}