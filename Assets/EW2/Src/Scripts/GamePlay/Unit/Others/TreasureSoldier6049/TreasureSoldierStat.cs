namespace EW2
{
    public class TreasureSoldierStat : RPGStatCollection
    {
        public TreasureSoldierStat(Unit unit) : base(unit)
        {
            var moveSpeed = CreateOrGetStat<MoveSpeed>(RPGStatType.MoveSpeed);
            moveSpeed.StatBaseValue = 1.5f;
        }
    }
}