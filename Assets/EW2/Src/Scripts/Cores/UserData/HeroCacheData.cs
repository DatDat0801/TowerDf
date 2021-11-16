namespace EW2
{
    public class HeroCacheData
    {
        private HeroStats statData;

        public HeroStats StatData => statData;

        private HeroStats statDataNextLvl;

        public HeroStats StatDataNextLvl => statDataNextLvl;

        private HeroItem heroItemData;

        public HeroItem HeroItemData => heroItemData;

        private HeroData heroDataBase;

        private int heroLevel;

        private int heroId;

        public int HeroId => heroId;

        public void InitData(int heroId)
        {
            this.heroId = heroId;

            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(heroId))
            {
                heroItemData = UserData.Instance.UserHeroData.GetHeroById(heroId);
            }
            else
            {
                heroItemData = new HeroItem(heroId);
            }

            heroLevel = heroItemData.level;

            heroDataBase = GameContainer.Instance.GetHeroData(heroId);

            if (heroDataBase)
            {
                var heroStat = heroDataBase.stats[heroLevel - 1];

                statData = new HeroStats(null, heroStat);

                var lvlNext = heroLevel < heroDataBase.stats.Length ? heroLevel : heroLevel - 1;

                var heroStatNext = heroDataBase.stats[lvlNext];

                statDataNextLvl = new HeroStats(null, heroStatNext);
            }

            UpgradeStatDataFlowSkill();
        }

        public void UpdateData()
        {
            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(heroId))
            {
                heroItemData = UserData.Instance.UserHeroData.GetHeroById(heroId);
            }
            else
            {
                heroItemData = new HeroItem(heroId);
            }

            heroLevel = heroItemData.level;

            var heroStat = heroDataBase.stats[heroLevel - 1];

            statData.UpdateStats(heroStat);

            var lvlNext = heroLevel < heroDataBase.stats.Length ? heroLevel : heroLevel - 1;

            var heroStatNext = heroDataBase.stats[lvlNext];

            statDataNextLvl.UpdateStats(heroStatNext);
        }

        public void ResetStat()
        {
            var heroStat = heroDataBase.stats[heroLevel - 1];

            statData.ClearStatModifiers(true);

            statData.UpdateStats(heroStat);
        }

        public void InsertData(HeroCacheData data)
        {
            heroId = data.heroId;

            heroLevel = data.heroLevel;

            heroDataBase = data.heroDataBase;

            heroItemData = data.HeroItemData;

            var heroStat = heroDataBase.stats[heroLevel - 1];

            statData = new HeroStats(null, heroStat);

            InsertDataStat(data.StatData);
        }

        private void InsertDataStat(HeroStats data)
        {
            var healthPoint = statData.GetStat(RPGStatType.Health);
            healthPoint.StatBaseValue = data.GetStat(RPGStatType.Health).StatValue;

            var moveSpeed = statData.GetStat(RPGStatType.MoveSpeed);
            moveSpeed.StatBaseValue = data.GetStat(RPGStatType.MoveSpeed).StatValue;

            var attackSpeed = statData.GetStat(RPGStatType.AttackSpeed);
            attackSpeed.StatBaseValue = data.GetStat(RPGStatType.AttackSpeed).StatValue;

            var armor = statData.GetStat(RPGStatType.Armor);
            armor.StatBaseValue = data.GetStat(RPGStatType.Armor).StatValue;

            var resistance = statData.GetStat(RPGStatType.Resistance);
            resistance.StatBaseValue = data.GetStat(RPGStatType.Resistance).StatValue;

            var damage = statData.GetStat(RPGStatType.Damage);
            damage.StatBaseValue = data.GetStat(RPGStatType.Damage).StatValue;

            var critChance = statData.GetStat(RPGStatType.CritChance);
            critChance.StatBaseValue = data.GetStat(RPGStatType.CritChance).StatValue;

            var critDamage = statData.GetStat(RPGStatType.CritDamage);
            critDamage.StatBaseValue = data.GetStat(RPGStatType.CritDamage).StatValue;

            var hpRegeneration = statData.GetStat(RPGStatType.HpRegeneration);
            hpRegeneration.StatBaseValue = data.GetStat(RPGStatType.HpRegeneration).StatValue;

            var timeRevive = statData.GetStat(RPGStatType.TimeRevive);
            timeRevive.StatBaseValue = data.GetStat(RPGStatType.TimeRevive).StatValue;

            var blockEnemyStat = statData.GetStat(RPGStatType.BlockEnemy);
            blockEnemyStat.StatBaseValue = data.GetStat(RPGStatType.BlockEnemy).StatValue;

            var cooldownRedutionStat = statData.GetStat(RPGStatType.CooldownReduction);
            cooldownRedutionStat.StatBaseValue = 0;

            var lifeStealStat = statData.GetStat(RPGStatType.LifeSteal);
            lifeStealStat.StatBaseValue = 0;

            var rangeDetectStat = statData.GetStat(RPGStatType.RangeDetect);
            rangeDetectStat.StatBaseValue = data.GetStat(RPGStatType.RangeDetect).StatValue;
        }

        private void UpgradeStatDataFlowSkill()
        {
            for (int i = 0; i < heroItemData.levelSkills.Length; i++)
            {
                var levelSkill = heroItemData.levelSkills[i];

                if (levelSkill <= 0) continue;

                if (this.heroDataBase != null)
                {
                    var statModifier = heroDataBase.GetStatModifierBySkill(i, levelSkill);

                    if (statModifier.Item1 != RPGStatType.None)
                    {
                        statData.ClearStatModifier(statModifier.Item1, true);

                        statData.AddStatModifier(statModifier.Item1, statModifier.Item2);

                        statData.UpdateStatModifer(statModifier.Item1);
                    }
                }
            }
        }
    }
}