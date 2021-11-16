using UnityEngine;

namespace EW2
{
    public static class BuffEnemyHelper
    {
        public static void BuffEnemyTournament(this EnemyBase enemy, TournamentEnemyConfig config)
        {
            //buff armor
            var armorModifiable = Ultilities.GetStatModifiable(RPGStatType.Armor);
            var armorModifier = new RPGStatModifier(armorModifiable, ModifierType.TotalPercent,
                config.armorRatioIncrease, false);
            enemy.Stats.AddStatModifier(RPGStatType.Armor, armorModifier);

            //damage
            var damageModifiable = Ultilities.GetStatModifiable(RPGStatType.Damage);
            var damageModifier = new RPGStatModifier(damageModifiable, ModifierType.TotalPercent,
                config.damageRatioIncrease, false);
            enemy.Stats.AddStatModifier(RPGStatType.Damage, damageModifier);
            //hp
            var hpModifiable = Ultilities.GetStatModifiable(RPGStatType.Health);
            var hpModifier =
                new RPGStatModifier(hpModifiable, ModifierType.TotalPercent, config.hpRatioIncrease, false);
            enemy.Stats.AddStatModifier(RPGStatType.Health, hpModifier);
            //resistance
            var resModifiable = Ultilities.GetStatModifiable(RPGStatType.Resistance);
            var resModifier = new RPGStatModifier(resModifiable, ModifierType.TotalPercent,
                config.resistanceRatioIncrease, false);
            enemy.Stats.AddStatModifier(RPGStatType.Resistance, resModifier);
            //crit
            var critDamageModifiable = Ultilities.GetStatModifiable(RPGStatType.CritDamage);
            var critDamageModifier = new RPGStatModifier(critDamageModifiable, ModifierType.TotalPercent,
                config.critDamageRatioIncrease, false);
            enemy.Stats.AddStatModifier(RPGStatType.CritDamage, critDamageModifier);
        }

        /// <summary>
        /// Buff for enemy
        /// growthValue = a(1+r)^x
        /// where 'a' is start value, 'r' percentage each loop, 'x' is loop time 
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="config"></param>
        /// <param name="times"></param>
        public static void BuffEnemyTournamentGrowth(this EnemyBase enemy, TournamentEnemyConfig config, int times)
        {
            if (times == 0) return;
            //buff armor
            var armorBaseValue = enemy.Stats.GetStat(RPGStatType.Armor).StatBaseValue;
            var armorGrowthValue = armorBaseValue * Mathf.Pow( (1 + config.armorRatioIncrease), times) - armorBaseValue;
            var armorModifiable = Ultilities.GetStatModifiable(RPGStatType.Armor);
            var armorModifier = new RPGStatModifier(armorModifiable, ModifierType.TotalAdd, armorGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Armor, armorModifier);

            //damage
            var damageBaseValue = enemy.Stats.GetStat(RPGStatType.Damage).StatBaseValue;
            var damageGrowthValue = damageBaseValue * Mathf.Pow( (1 + config.damageRatioIncrease), times) - damageBaseValue;
            var damageModifiable = Ultilities.GetStatModifiable(RPGStatType.Damage);
            var damageModifier = new RPGStatModifier(damageModifiable, ModifierType.TotalAdd, damageGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Damage, damageModifier);
            //hp
            var hpBaseValue = enemy.Stats.GetStat(RPGStatType.Health).StatBaseValue;
            var hpGrowthValue = hpBaseValue * Mathf.Pow( (1 + config.hpRatioIncrease), times) - hpBaseValue;
            var hpModifiable = Ultilities.GetStatModifiable(RPGStatType.Health);
            var hpModifier = new RPGStatModifier(hpModifiable, ModifierType.TotalAdd, hpGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Health, hpModifier);
            //resistance
            var resBaseValue = enemy.Stats.GetStat(RPGStatType.Resistance).StatBaseValue;
            var resGrowthValue = resBaseValue * Mathf.Pow( (1 + config.resistanceRatioIncrease), times) - resBaseValue;
            var resModifiable = Ultilities.GetStatModifiable(RPGStatType.Resistance);
            var resModifier = new RPGStatModifier(resModifiable, ModifierType.TotalAdd, resGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Resistance, resModifier);
            //crit
            var critBaseValue = enemy.Stats.GetStat(RPGStatType.CritDamage).StatBaseValue;
            var critGrowthValue = critBaseValue * Mathf.Pow( (1 + config.critDamageRatioIncrease), times) - critBaseValue;
            var critDamageModifiable = Ultilities.GetStatModifiable(RPGStatType.CritDamage);
            var critDamageModifier = new RPGStatModifier(critDamageModifiable, ModifierType.TotalAdd, critGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.CritDamage, critDamageModifier);
        }
                /// <summary>
        /// Buff for enemy
        /// growthValue = a(1+r)^x
        /// where 'a' is start value, 'r' percentage each loop, 'x' is loop time 
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="config"></param>
        /// <param name="times"></param>
        public static void BuffEnemyDefensiveMode(this EnemyBase enemy, DefensiveEnemyConfig config, int times)
        {
            if (times == 0) return;
            //buff armor
            var armorBaseValue = enemy.Stats.GetStat(RPGStatType.Armor).StatBaseValue;
            var armorGrowthValue = armorBaseValue * Mathf.Pow( (1 + config.armorRatioIncrease), times) - armorBaseValue;
            var armorModifiable = Ultilities.GetStatModifiable(RPGStatType.Armor);
            var armorModifier = new RPGStatModifier(armorModifiable, ModifierType.TotalAdd, armorGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Armor, armorModifier);

            //damage
            var damageBaseValue = enemy.Stats.GetStat(RPGStatType.Damage).StatBaseValue;
            var damageGrowthValue = damageBaseValue * Mathf.Pow( (1 + config.damageRatioIncrease), times) - damageBaseValue;
            var damageModifiable = Ultilities.GetStatModifiable(RPGStatType.Damage);
            var damageModifier = new RPGStatModifier(damageModifiable, ModifierType.TotalAdd, damageGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Damage, damageModifier);
            //hp
            var hpBaseValue = enemy.Stats.GetStat(RPGStatType.Health).StatBaseValue;
            var hpGrowthValue = hpBaseValue * Mathf.Pow( (1 + config.hpRatioIncrease), times) - hpBaseValue;
            var hpModifiable = Ultilities.GetStatModifiable(RPGStatType.Health);
            var hpModifier = new RPGStatModifier(hpModifiable, ModifierType.TotalAdd, hpGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Health, hpModifier);
            //resistance
            var resBaseValue = enemy.Stats.GetStat(RPGStatType.Resistance).StatBaseValue;
            var resGrowthValue = resBaseValue * Mathf.Pow( (1 + config.resistanceRatioIncrease), times) - resBaseValue;
            var resModifiable = Ultilities.GetStatModifiable(RPGStatType.Resistance);
            var resModifier = new RPGStatModifier(resModifiable, ModifierType.TotalAdd, resGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.Resistance, resModifier);
            //crit
            var critBaseValue = enemy.Stats.GetStat(RPGStatType.CritDamage).StatBaseValue;
            var critGrowthValue = critBaseValue * Mathf.Pow( (1 + config.critDamageRatioIncrease), times) - critBaseValue;
            var critDamageModifiable = Ultilities.GetStatModifiable(RPGStatType.CritDamage);
            var critDamageModifier = new RPGStatModifier(critDamageModifiable, ModifierType.TotalAdd, critGrowthValue, false);
            enemy.Stats.AddStatModifier(RPGStatType.CritDamage, critDamageModifier);
        }
    }
}