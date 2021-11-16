namespace EW2
{
    public class Tower2002Stats : TowerStats
    {
        public Tower2002Stats(Unit unit, TowerStatBase stats) : base(unit, stats)
        {
            
        }
        
        public override void ConfigureStats()
        {
            ClearStatModifiers();
            
            CreateDefaultStats();
            
            CreateDamageReduceBullet();
            
            UpgradeTower();
        }

        private void CreateDamageReduceBullet()
        {
            var bulletData = ((Tower2002TowerStatBase) this.stats).bulletData;
            var damageReduceBullet = CreateOrGetStat<DamageReduceBullet>(RPGStatType.DamageReduceBullet);
            damageReduceBullet.StatBaseValue = bulletData.damageReduceBullet;
        }

        protected sealed override void UpgradeTower()
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(owner.Id);
            if (towerStat == null)
                return;
            
            var upgradeLevel = towerStat.towerLevel;
            
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(upgradeLevel);

            var damage = GetStat<Damage>(RPGStatType.Damage);
            damage.AddModifier(new RPGStatModifier(damage, ModifierType.BasePercent, totalStat.bonusMagicDamage));

            var rangeDetect = GetStat<RangeDetect>(RPGStatType.RangeDetect);
            rangeDetect.AddModifier(new RPGStatModifier(rangeDetect, ModifierType.BasePercent, totalStat.bonusDetectRange));
            
            var damageReduceBullet = GetStat<DamageReduceBullet>(RPGStatType.DamageReduceBullet);
            damageReduceBullet.AddModifier(new RPGStatModifier(damageReduceBullet, ModifierType.BaseAdd, totalStat.damageReduceBulletDecrease));
        }
    }
}