namespace EW2
{
    public class CavalryStat : RPGStatCollection
    {
        public CavalryStat(Unit unit, Cavalry6050Data.DataCavalry cavalryData) : base(unit)
        {
            var moveSpeed = CreateOrGetStat<MoveSpeed>(RPGStatType.MoveSpeed);
            moveSpeed.StatBaseValue = cavalryData.moveSpeed;

            var damage = CreateOrGetStat<Damage>(RPGStatType.Damage);
            damage.StatBaseValue = cavalryData.damage;
            damage.damageType = cavalryData.damageType;
            
            var critChance = CreateOrGetStat<CritChance>(RPGStatType.CritChance);
            critChance.StatBaseValue = 0;
            
            var critDamage = CreateOrGetStat<CritDamage>(RPGStatType.CritDamage);
            critDamage.StatBaseValue = 0;
        }
    }
}