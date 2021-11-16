using Zitga.Observables;

namespace EW2
{
    public class AttackBuff
    {
        private readonly Unit _unit;
        private RPGStatModifier _damage;

        public AttackBuff(Unit unit)
        {
            this._unit = unit;
        }

        private void Restore()
        {
            if (this._damage != null)
            {
                this._unit.Stats.AddStatModifier(RPGStatType.Damage, this._damage);
            }
        }

        public void Buff(ModifierType modifierType, float value)
        {
            if (value <= 0) return;
            var modifiable = Ultilities.GetStatModifiable(RPGStatType.Damage);
            this._damage = new RPGStatModifier(modifiable, modifierType, value, false);
            this._unit.Stats.AddStatModifier(RPGStatType.Damage, this._damage);
        }

        public void Nerf()
        {
            this._unit.Stats.RemoveStatModifier(RPGStatType.Damage, this._damage, true);
        }
    }
}