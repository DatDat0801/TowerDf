using EW2.Spell;

namespace EW2
{
    public class SpellStats : RPGStatCollection
    {
        private readonly SpellStatBase _spellStat;

        public SpellStats(Unit unit, SpellStatBase spellStat) : base(unit)
        {
            this._spellStat = spellStat;
            ConfigureStats();
        }

        public sealed override void ConfigureStats()
        {
            var healthPoint = CreateOrGetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.StatBaseValue = this._spellStat.hp;
            healthPoint.CurrentValue = this._spellStat.hp;

            var damage = CreateOrGetStat<Damage>(RPGStatType.Damage);
            damage.StatBaseValue = this._spellStat.damage;
            damage.damageType = this._spellStat.damageType;
            
            var armor = CreateOrGetStat<ArmorPhysical>(RPGStatType.Armor);
            armor.StatBaseValue = 0;
            
            var resistance = CreateOrGetStat<ArmorMagical>(RPGStatType.Resistance);
            resistance.StatBaseValue = 0;
        }
    }
}