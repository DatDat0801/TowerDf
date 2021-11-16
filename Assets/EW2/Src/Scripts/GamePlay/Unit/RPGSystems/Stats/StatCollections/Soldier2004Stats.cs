namespace EW2
{
    public class Soldier2004Stats : RPGStatCollection
    {
        private TowerData2004.Soldier2004Data stats;

        public Soldier2004Stats(Unit unit, TowerData2004.Soldier2004Data stats) : base(unit)
        {
            UpdateStats(stats);
        }
        
        public void UpdateStats(TowerData2004.Soldier2004Data stats)
        {
            this.stats = stats;
            
            ConfigureStats();
        }

        public override void ConfigureStats()
        {
            ClearStatModifiers();
            
            var healthPoint = CreateOrGetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.StatBaseValue = this.stats.health;
            healthPoint.CurrentValue = this.stats.health;

            var moveSpeed = CreateOrGetStat<MoveSpeed>(RPGStatType.MoveSpeed);
            moveSpeed.StatBaseValue = this.stats.moveSpeed;

            var attackSpeed = CreateOrGetStat<AttackSpeed>(RPGStatType.AttackSpeed);
            attackSpeed.StatBaseValue = this.stats.attackSpeed;

            var armor = CreateOrGetStat<ArmorPhysical>(RPGStatType.Armor);
            armor.StatBaseValue = this.stats.armor;

            var resistance = CreateOrGetStat<ArmorMagical>(RPGStatType.Resistance);
            resistance.StatBaseValue = this.stats.resistance;

            var damage = CreateOrGetStat<Damage>(RPGStatType.Damage);
            damage.StatBaseValue = this.stats.damage;
            damage.damageType = this.stats.damageType;

            var critChance = CreateOrGetStat<CritChance>(RPGStatType.CritChance);
            critChance.StatBaseValue = this.stats.critChance;

            var critDamage = CreateOrGetStat<CritDamage>(RPGStatType.CritDamage);
            critDamage.StatBaseValue = this.stats.critDamage;

            var hpRegeneration = CreateOrGetStat<HpRegeneration>(RPGStatType.HpRegeneration);
            hpRegeneration.StatBaseValue = this.stats.hpRegeneration;

            var timeRevive = CreateOrGetStat<TimeRevive>(RPGStatType.TimeRevive);
            timeRevive.StatBaseValue = this.stats.timeRevive;

            var blockEnemyStat = CreateOrGetStat<BlockEnemyStat>(RPGStatType.BlockEnemy);
            blockEnemyStat.StatBaseValue = this.stats.blockEnemy;
            
            UpgradeTower();
        }
        
        protected void UpgradeTower()
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(owner.Id);
            if (towerStat == null)
                return;
            
            var upgradeLevel = towerStat.towerLevel;
            
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2004(upgradeLevel);

            var damage = GetStat<Damage>(RPGStatType.Damage);
            damage.AddModifier(new RPGStatModifier(damage, ModifierType.BasePercent, totalStat.bonusDamage));

            var health = GetStat<HealthPoint>(RPGStatType.Health);
            var deltaHealth = health.StatValue;
            health.AddModifier(new RPGStatModifier(health, ModifierType.BasePercent, totalStat.bonusHeath));
            deltaHealth = health.StatValue - deltaHealth;
            health.CurrentValue += deltaHealth;
            
            var timeRevive = GetStat<TimeRevive>(RPGStatType.TimeRevive);
            timeRevive.AddModifier(new RPGStatModifier(timeRevive, ModifierType.BasePercent, totalStat.bonusSpawnTime));
        }
    }
}