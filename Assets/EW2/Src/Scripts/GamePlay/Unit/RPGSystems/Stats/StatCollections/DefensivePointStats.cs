namespace EW2
{
    public class DefensivePointStats : RPGStatCollection
    {
        private readonly DefensivePointStatBase _statBase;
        public DefensivePointStats(Unit unit, DefensivePointStatBase stat) : base(unit)
        {
            this._statBase = stat;
            ConfigureStats();
        }
        public sealed override void ConfigureStats()
        {
            var healthPoint = CreateOrGetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.StatBaseValue = this._statBase.hp;
            healthPoint.CurrentValue = this._statBase.hp;
            
            var armor = CreateOrGetStat<ArmorPhysical>(RPGStatType.Armor);
            armor.StatBaseValue = 0;
            
            var resistance = CreateOrGetStat<ArmorMagical>(RPGStatType.Resistance);
            resistance.StatBaseValue = 0;
        }
    }
}