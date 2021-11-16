namespace EW2
{
    public class Tower2001Stats : TowerStats
    {
        public Tower2001Stats(Unit unit, TowerStatBase stats) : base(unit, stats)
        {
        }

        protected sealed override void UpgradeTower()
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(owner.Id);
            if (towerStat == null)
                return;
            
            var upgradeLevel = towerStat.towerLevel;
            
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2001(upgradeLevel);

            var damage = GetStat<Damage>(RPGStatType.Damage);
            damage.AddModifier(new RPGStatModifier(damage, ModifierType.BasePercent, totalStat.bonusDamage));
            
            var critDamage = GetStat<CritDamage>(RPGStatType.CritDamage);
            critDamage.AddModifier(new RPGStatModifier(critDamage, ModifierType.BasePercent, totalStat.bonusCrit));
            
            var rangeDetect = GetStat<RangeDetect>(RPGStatType.RangeDetect);
            rangeDetect.AddModifier(new RPGStatModifier(rangeDetect, ModifierType.BasePercent, totalStat.bonusDetectRange));
        }
    }
}