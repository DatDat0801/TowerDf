namespace EW2
{
    public class DefenseBuff
    {
        private readonly Unit _unit;
        private RPGStatModifier _armor;
        private RPGStatModifier _resistance;

        public DefenseBuff(Unit unit)
        {
            this._unit = unit;
        }
        private void Restore()
        {
            if (this._armor != null)
            {
                this._unit.Stats.AddStatModifier(RPGStatType.Armor, this._armor);
            }

            if (this._resistance != null)
            {
                this._unit.Stats.AddStatModifier(RPGStatType.Resistance, this._resistance);
            }
        }
        public void Buff(ModifierType modifierType, float armor, float damageResistance)
        {
            var armorModifiable = Ultilities.GetStatModifiable(RPGStatType.Armor);
            this._armor = new RPGStatModifier(armorModifiable, modifierType, armor, false);
            this._unit.Stats.AddStatModifier(RPGStatType.Armor, this._armor);

            var resistanceModifiable = Ultilities.GetStatModifiable(RPGStatType.Resistance);
            _resistance = new RPGStatModifier(resistanceModifiable, modifierType, damageResistance, false);
            this._unit.Stats.AddStatModifier(RPGStatType.Resistance, _resistance);
        }

        public void Nerf()
        {
            this._unit.Stats.RemoveStatModifier(RPGStatType.Armor, this._armor, true);
            this._unit.Stats.RemoveStatModifier(RPGStatType.Resistance, _resistance, true);
        }
    }
}