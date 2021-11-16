namespace EW2
{
    public class EnemyStats : RPGStatCollection
    {
        private EnemyStatBase stats;

        public EnemyStats(Unit unit, EnemyStatBase stats) : base(unit)
        {
            this.stats = stats;

            ConfigureStats();
        }

        public override void ConfigureStats()
        {
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


            var critDamageBonus = CreateOrGetStat<CritDamageBonus>(RPGStatType.CritDamageBonus);
            critDamageBonus.StatBaseValue = 0;

        }
    }
}
