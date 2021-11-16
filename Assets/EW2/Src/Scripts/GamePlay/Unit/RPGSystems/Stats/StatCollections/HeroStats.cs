using System.Collections.Generic;


namespace EW2
{
    public class HeroStats : RPGStatCollection
    {
        private HeroStatBase stats;
        private List<int> runesCache = new List<int>();

        private Dictionary<int, Dictionary<RPGStatType, RPGStatModifier>> dictStatRuneApply;
        private Dictionary<int, Dictionary<RPGStatType, RPGStatModifier>> dictStatSetRuneApply;

        public HeroStats(Unit unit, HeroStatBase stats) : base(unit)
        {
            this.stats = stats;

            ConfigureStats();

            InsertStatByRune();

            InsertStatBySetRune();
        }

        public void UpdateStats(HeroStatBase stats)
        {
            this.stats = stats;

            ConfigureStats();

            UpdateStatByRune();

            UpdateStatBySetRune();
        }

        public void RefresStats()
        {
            InsertStatByRune();
            InsertStatBySetRune();
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

            var hpRegeneration = CreateOrGetStat<HpRegeneration>(RPGStatType.HpRegeneration);
            hpRegeneration.StatBaseValue = this.stats.hpRegeneration;

            var timeRevive = CreateOrGetStat<TimeRevive>(RPGStatType.TimeRevive);
            timeRevive.StatBaseValue = this.stats.timeRevive;

            var blockEnemyStat = CreateOrGetStat<BlockEnemyStat>(RPGStatType.BlockEnemy);
            blockEnemyStat.StatBaseValue = this.stats.blockEnemy;

            var cooldownRedutionStat = CreateOrGetStat<CooldownReduction>(RPGStatType.CooldownReduction);
            cooldownRedutionStat.StatBaseValue = 0;
            

            var lifeStealStat = CreateOrGetStat<LifeSteal>(RPGStatType.LifeSteal);
            lifeStealStat.StatBaseValue = 0;

            var rangeDetectStat = CreateOrGetStat<RangeDetect>(RPGStatType.RangeDetect);
            rangeDetectStat.StatBaseValue = this.stats.detectRangeAttack;
        }
        /// <summary>
        /// make a custom cooldown stat
        /// </summary>
        public void AddCustomCooldownStat(float statBaseValue)
        {
            var skillActiveCooldown = CreateOrGetStat<SkillActiveCooldown>(RPGStatType.SkillActiveCooldown);
            skillActiveCooldown.StatBaseValue = statBaseValue;

        }

        private void InsertStatByRune()
        {
            if (dictStatRuneApply == null)
                dictStatRuneApply = new Dictionary<int, Dictionary<RPGStatType, RPGStatModifier>>();

            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(stats.id))
            {
                var heroItem = UserData.Instance.UserHeroData.GetHeroById(stats.id);
                if (heroItem != null)
                {
                    runesCache.Clear();
                    runesCache.AddRange(heroItem.runeEquips);

                    for (int i = 0; i < runesCache.Count; i++)
                    {
                        if (heroItem.runeEquips[i] >= 0)
                        {
                            var runeItem =
                                (RuneItem) UserData.Instance.GetInventory(InventoryType.Rune, heroItem.runeEquips[i]);
                            var lstStats = runeItem.AddStatRune(this);
                            dictStatRuneApply[i] = lstStats;
                        }
                    }
                }
            }
        }

        private void UpdateStatByRune()
        {
            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(stats.id))
            {
                var heroItem = UserData.Instance.UserHeroData.GetHeroById(stats.id);
                if (heroItem != null)
                {
                    for (int i = 0; i < heroItem.runeEquips.Length; i++)
                    {
                        if (heroItem.runeEquips[i] >= 0)
                        {
                            // remove old stat
                            if (dictStatRuneApply.ContainsKey(i))
                            {
                                var statsList = dictStatRuneApply[i];
                                foreach (var stat in statsList)
                                {
                                    RemoveStatModifier(stat.Key, stat.Value, true);
                                }
                            }

                            // add new stat
                            var runeItem =
                                (RuneItem) UserData.Instance.GetInventory(InventoryType.Rune, heroItem.runeEquips[i]);
                            var lstStats = runeItem.AddStatRune(this);
                            dictStatRuneApply[i] = lstStats;
                        }
                        else if (heroItem.runeEquips[i] < 0 && runesCache[i] >= 0)
                        {
                            // remove stat
                            var statsList = dictStatRuneApply[i];
                            foreach (var stat in statsList)
                            {
                                RemoveStatModifier(stat.Key, stat.Value, true);
                            }

                            dictStatRuneApply.Remove(i);
                        }

                        runesCache[i] = heroItem.runeEquips[i];
                    }
                }
            }
        }

        private void InsertStatBySetRune()
        {
            if (dictStatSetRuneApply == null)
                dictStatSetRuneApply = new Dictionary<int, Dictionary<RPGStatType, RPGStatModifier>>();

            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(stats.id))
            {
                var heroItem = UserData.Instance.UserHeroData.GetHeroById(stats.id);
                if (heroItem != null)
                {
                    foreach (var setRune in heroItem.dictRuneSets)
                    {
                        var lstStats = setRune.Value.AddStatRune(this);
                        dictStatSetRuneApply[setRune.Key] = lstStats;
                    }
                }
            }
        }

        private void UpdateStatBySetRune()
        {
            if (UserData.Instance.UserHeroData.CheckHeroUnlocked(stats.id))
            {
                var heroItem = UserData.Instance.UserHeroData.GetHeroById(stats.id);
                if (heroItem != null)
                {
                    foreach (var setRuneApply in dictStatSetRuneApply)
                    {
                        var statsOldList = setRuneApply.Value;
                        foreach (var stat in statsOldList)
                        {
                            RemoveStatModifier(stat.Key, stat.Value, true);
                        }
                    }

                    dictStatSetRuneApply.Clear();

                    foreach (var setRune in heroItem.dictRuneSets)
                    {
                        var lstStats = setRune.Value.AddStatRune(this);
                        dictStatSetRuneApply[setRune.Key] = lstStats;
                    }
                }
            }
        }
    }
}