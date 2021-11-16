namespace EW2
{
    public abstract class TowerStats : RPGStatCollection
    {
        protected TowerStatBase stats;

        public TowerStats(Unit unit, TowerStatBase stats) : base(unit)
        {
            UpdateStats(stats);
        }

        public void UpdateStats(TowerStatBase stats)
        {
            this.stats = stats;

            ConfigureStats();
        }

        public override void ConfigureStats()
        {
            ClearStatModifiers();
            
            CreateDefaultStats();
            
            UpgradeTower();
        }

        protected virtual void CreateDefaultStats()
        {
            var damage = CreateOrGetStat<Damage>(RPGStatType.Damage);
            damage.StatBaseValue = this.stats.damage;
            damage.damageType = this.stats.damageType;

            var attackSpeed = CreateOrGetStat<AttackSpeed>(RPGStatType.AttackSpeed);
            attackSpeed.StatBaseValue = this.stats.attackSpeed;

            var critChance = CreateOrGetStat<CritChance>(RPGStatType.CritChance);
            critChance.StatBaseValue = this.stats.critChance;

            var critDamage = CreateOrGetStat<CritDamage>(RPGStatType.CritDamage);
            critDamage.StatBaseValue = this.stats.critDamage;
            
            var rangeDetectStat = CreateOrGetStat<RangeDetect>(RPGStatType.RangeDetect);
            rangeDetectStat.StatBaseValue = this.stats.detectRangeAttack;
        }

        protected abstract void UpgradeTower();
    }
}