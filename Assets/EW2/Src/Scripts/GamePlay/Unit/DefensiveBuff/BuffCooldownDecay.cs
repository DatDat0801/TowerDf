using UnityEngine;

namespace EW2
{
    public class BuffCooldownDecay
    {
        private readonly HeroBase _unit;
        
        private RPGStatModifier _cooldownReduction;
        public BuffCooldownDecay(HeroBase unit)
        {
            this._unit = unit;
        }
        
        public void Buff(ModifierType modifierType, float value)
        {
            if (value == 0) return;
            var modifiable = Ultilities.GetStatModifiable(RPGStatType.CooldownReduction);
            _cooldownReduction = new RPGStatModifier(modifiable, modifierType, value, false);
            this._unit.Stats.AddStatModifier(RPGStatType.CooldownReduction, this._cooldownReduction);
        }
    }
}