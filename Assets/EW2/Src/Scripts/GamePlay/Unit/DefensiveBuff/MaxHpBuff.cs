namespace EW2
{
    public class MaxHpBuff
    {
        private readonly Unit _unit;
        private RPGStatModifier _hp;

        public MaxHpBuff(Unit unit)
        {
            this._unit = unit;
        }

        private void Restore()
        {
            if (this._hp != null)
            {
                this._unit.Stats.AddStatModifier(RPGStatType.Health, this._hp);
            }
        }

        public void Buff(ModifierType modifierType, float value)
        {
            if (value <= 0) return;
            var modifiable = Ultilities.GetStatModifiable(RPGStatType.Health);
            this._hp = new RPGStatModifier(modifiable, modifierType, value, false);
            this._unit.Stats.AddStatModifier(RPGStatType.Health, this._hp);
            
        }

        public void Nerf()
        {
            this._unit.Stats.RemoveStatModifier(RPGStatType.Health, this._hp, true);
        }
    }
}