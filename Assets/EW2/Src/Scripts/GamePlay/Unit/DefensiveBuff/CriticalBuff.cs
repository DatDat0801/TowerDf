namespace EW2
{
    public class CriticalBuff
    {
        private readonly Unit _unit;
        private RPGStatModifier _critical;

        public CriticalBuff(Unit unit)
        {
            this._unit = unit;
        }

        private void Restore()
        {
            if (this._critical != null)
            {
                this._unit.Stats.AddStatModifier(RPGStatType.CritDamageBonus, this._critical);
            }
        }

        public void Buff(ModifierType modifierType, float value)
        {
            if (value <= 0) return;
            var modifiable = Ultilities.GetStatModifiable(RPGStatType.CritDamageBonus);
            this._critical = new RPGStatModifier(modifiable, modifierType, value, true);
            this._unit.Stats.AddStatModifier(RPGStatType.CritDamageBonus, this._critical);
        }

        public void Nerf()
        {
            this._unit.Stats.RemoveStatModifier(RPGStatType.CritDamageBonus, this._critical, true);
        }
    }
}