namespace EW2.Spell
{
    public sealed class Warrior4004Stats : RPGStatCollection
    {
        private Warrior4004Stat _warrior4004Stat;
        public Warrior4004Stats(Unit unit, Warrior4004Stat warriorStat) : base(unit)
        {

            this._warrior4004Stat = warriorStat;
            ConfigureStats();
        }
        
         public override void ConfigureStats()
        {
            var healthPoint = CreateOrGetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.StatBaseValue = this._warrior4004Stat.hp;
            healthPoint.CurrentValue = this._warrior4004Stat.hp;

            var moveSpeed = CreateOrGetStat<MoveSpeed>(RPGStatType.MoveSpeed);
            moveSpeed.StatBaseValue = this._warrior4004Stat.moveSpeed;

            var attackSpeed = CreateOrGetStat<AttackSpeed>(RPGStatType.AttackSpeed);
            attackSpeed.StatBaseValue = this._warrior4004Stat.attackSpeed;

            var armor = CreateOrGetStat<ArmorPhysical>(RPGStatType.Armor);
            armor.StatBaseValue = this._warrior4004Stat.armor;

            var resistance = CreateOrGetStat<ArmorMagical>(RPGStatType.Resistance);
            resistance.StatBaseValue = this._warrior4004Stat.resistance;

            var damage = CreateOrGetStat<Damage>(RPGStatType.Damage);
            damage.StatBaseValue = this._warrior4004Stat.damage;
            damage.damageType = this._warrior4004Stat.damageType;

            var critChance = CreateOrGetStat<CritChance>(RPGStatType.CritChance);
            critChance.StatBaseValue = this._warrior4004Stat.critChance;

            var critDamage = CreateOrGetStat<CritDamage>(RPGStatType.CritDamage);
            critDamage.StatBaseValue = this._warrior4004Stat.critDamage;

            var blockEnemyStat = CreateOrGetStat<BlockEnemyStat>(RPGStatType.BlockEnemy);
            blockEnemyStat.StatBaseValue = this._warrior4004Stat.blockEnemy;

            var cooldownRedutionStat = CreateOrGetStat<CooldownReduction>(RPGStatType.CooldownReduction);
            cooldownRedutionStat.StatBaseValue = 0;

            var lifeStealStat = CreateOrGetStat<LifeSteal>(RPGStatType.LifeSteal);
            lifeStealStat.StatBaseValue = 0;

            // var rangeDetectStat = CreateOrGetStat<RangeDetect>(RPGStatType.RangeDetect);
            // rangeDetectStat.StatBaseValue = this._warrior4004Stat.detectRangeAttack;
        }
    }
}