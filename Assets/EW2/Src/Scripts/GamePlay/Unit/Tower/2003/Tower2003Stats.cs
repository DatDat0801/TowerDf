namespace EW2
{
    public class Tower2003Stats : TowerStats
    {
        public Tower2003Stats(Unit unit, TowerStatBase stats) : base(unit, stats)
        {
        }

        protected override void UpgradeTower()
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(owner.Id);
            if (towerStat == null)
                return;
            
            var upgradeLevel = towerStat.towerLevel;
            
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2003(upgradeLevel);

            var damage = GetStat<Damage>(RPGStatType.Damage);
            damage.AddModifier(new RPGStatModifier(damage, ModifierType.BasePercent, totalStat.bonusDamage));

            var rangeDetect = GetStat<RangeDetect>(RPGStatType.RangeDetect);
            rangeDetect.AddModifier(new RPGStatModifier(rangeDetect, ModifierType.BasePercent, totalStat.bonusDetectRange));
            
            var attackSpeed = GetStat<AttackSpeed>(RPGStatType.AttackSpeed);
            attackSpeed.AddModifier(new RPGStatModifier(attackSpeed, ModifierType.BasePercent, totalStat.bonusAttackSpeed));
        }
    }
}