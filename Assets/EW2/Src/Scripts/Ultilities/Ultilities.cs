using TigerForge;
using UnityEngine;
using Zitga.Localization;

namespace EW2
{
    public static class Ultilities
    {
        public static Color GetColorFromHtmlString(string hex)
        {
            var colorResult = new Color();

            ColorUtility.TryParseHtmlString(hex, out colorResult);

            return colorResult;
        }

        public static string GetDeviceUniqueIdentifier()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static string GetNameHero(int heroId)
        {
            return Localization.Current.Get("heroes", $"hero_name_{heroId.ToString()}");
        }

        public static string CalculateTooltipTutorial(int tutorialId)
        {
            return Localization.Current.Get("tutorial", $"tool_tip_{tutorialId.ToString()}");
        }

        public static string GetTagHero(int heroId)
        {
            return Localization.Current.Get("heroes", $"hero_tag_{heroId.ToString()}");
        }

        public static string GetHeroStory(int heroId)
        {
            return Localization.Current.Get("heroes", $"hero_story_{heroId.ToString()}");
        }

        public static string GetHeroDescSkill(int heroId, int skillId)
        {
            return Localization.Current.Get("heroes", $"hero_skill{heroId.ToString()}_des_{skillId.ToString()}");
        }

        public static string GetHeroSkillName(int heroId, int skillId)
        {
            return Localization.Current.Get("heroes", $"hero_nameskill{heroId.ToString()}_{skillId.ToString()}");
        }

        public static string GetNameStat(RPGStatType type)
        {
            return Localization.Current.Get("gameplay", $"name_{type.ToString().ToLower()}");
        }

        public static string GetHeroClasses(HeroClasses classes)
        {
            return Localization.Current.Get("heroes", $"hero_class_{classes.ToString().ToLower()}");
        }

        public static Color GetTextColorHeroClasses(HeroClasses classes)
        {
            Color textColor = new Color();

            switch (classes)
            {
                case HeroClasses.Melee:
                    ColorUtility.TryParseHtmlString("#ad252b", out textColor);
                    break;
                case HeroClasses.Ranged:
                    ColorUtility.TryParseHtmlString("#438b31", out textColor);
                    break;
                case HeroClasses.Assassin:
                    ColorUtility.TryParseHtmlString("#893da9", out textColor);
                    break;
                case HeroClasses.Aoe:
                    ColorUtility.TryParseHtmlString("#eb9b21", out textColor);
                    break;
                case HeroClasses.Tank:
                    ColorUtility.TryParseHtmlString("#338EDE", out textColor);
                    break;
                case HeroClasses.Summoner:
                    ColorUtility.TryParseHtmlString("#1dc0ac", out textColor);
                    break;
                case HeroClasses.Control:
                    ColorUtility.TryParseHtmlString("#82bda2", out textColor);
                    break;
                case HeroClasses.Buff:
                    ColorUtility.TryParseHtmlString("#ff5341", out textColor);
                    break;
                case HeroClasses.Debuff:
                    ColorUtility.TryParseHtmlString("#405367", out textColor);
                    break;
            }

            return textColor;
        }

        public static void ShowToastNoti(string content)
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, content);
        }

        public static string GetNameCurrency(int typeCurrency)
        {
            return Localization.Current.Get("currency_type", $"currency_{typeCurrency.ToString()}");
        }

        public static string GetNameSpell(int spellId)
        {
            return Localization.Current.Get("Spell", $"spell_name_{Ultilities.GetSpellIdFamily(spellId).ToString()}");
        }

        /// <summary>
        /// Get the root family of spell, for example 40031 has root 4003
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        public static int GetSpellIdFamily(int spellId) //40031
        {
            int r = spellId / 10;
            if (r < 4000)
            {
                return spellId;
            }

            return r;
        }

        public static string GetDescSpellSkillActive(int spellId)
        {
            return Localization.Current.Get("Spell",
                $"active_description_{Ultilities.GetSpellIdFamily(spellId).ToString()}");
        }

        public static string GetDescSpellSkillPassive(int spellId)
        {
            return Localization.Current.Get("Spell",
                $"passive_description_{Ultilities.GetSpellIdFamily(spellId).ToString()}");
        }

        public static string GetRarity(int rarity)
        {
            return Localization.Current.Get("gameplay", $"rarity_{rarity}");
        }

        public static string GetNameRune(int runeId)
        {
            var runeIdCompare = InventoryDataBase.GetRuneId(runeId);
            return Localization.Current.Get("rune", $"rune_name_{runeIdCompare.Item1}");
        }

        public static Color GetColorRarity(int rarity)
        {
            Color textColor = new Color();

            switch (rarity)
            {
                case 0:
                    ColorUtility.TryParseHtmlString("#609817", out textColor);
                    break;
                case 1:
                    ColorUtility.TryParseHtmlString("#2256A4", out textColor);
                    break;
                case 2:
                    ColorUtility.TryParseHtmlString("#A53CA9", out textColor);
                    break;
                case 3:
                    ColorUtility.TryParseHtmlString("#B38E0C", out textColor);
                    break;
                case 4:
                    ColorUtility.TryParseHtmlString("#BB7B36", out textColor);
                    break;
            }

            return textColor;
        }

        public static RPGStatModifiable GetStatModifiable(RPGStatType statType)
        {
            switch (statType)
            {
                case RPGStatType.Damage:
                    return new Damage();
                case RPGStatType.Armor:
                    return new ArmorPhysical();
                case RPGStatType.Health:
                    return new HealthPoint();
                case RPGStatType.Resistance:
                    return new ArmorMagical();
                case RPGStatType.AttackSpeed:
                    return new AttackSpeed();
                case RPGStatType.BlockEnemy:
                    return new BlockEnemyStat();
                case RPGStatType.CooldownReduction:
                    return new CooldownReduction();
                case RPGStatType.CritChance:
                    return new CritChance();
                case RPGStatType.CritDamage:
                    return new CritDamage();
                case RPGStatType.DetectBlock:
                    return new DetectBlock();
                case RPGStatType.HpRegeneration:
                    return new HpRegeneration();
                case RPGStatType.LifeSteal:
                    return new LifeSteal();
                case RPGStatType.MoveSpeed:
                    return new MoveSpeed();
                case RPGStatType.RangeDetect:
                    return new RangeDetect();
                case RPGStatType.TimeRevive:
                    return new TimeRevive();
                case RPGStatType.CritDamageBonus:
                    return new CritDamageBonus();
                default:
                    return null;
            }
        }

        public static int GetRarityMoney(int moneyType)
        {
            switch (moneyType)
            {
                case MoneyType.Crystal:
                case MoneyType.Stamina:
                case MoneyType.Exp:
                case MoneyType.KeyRuneBasic:
                case MoneyType.KeySpellBasic:
                    return 1;
                case MoneyType.ExpRune:
                case MoneyType.KeyRunePremium:
                case MoneyType.KeySpellPremium:
                    return 2;
                case MoneyType.Diamond:
                    return 3;
                default:
                    return 1;
            }
        }

        public static int GetRankTournamentId(int rank)
        {
            var rankId = 0;
            if (rank >= 0 && rank <= 3)
            {
                rankId = 7;
            }
            else if (rank > 3 && rank <= 10)
            {
                rankId = 6;
            }
            else if (rank > 10 && rank <= 20)
            {
                rankId = 5;
            }
            else if (rank > 20 && rank <= 50)
            {
                rankId = 4;
            }
            else if (rank > 50 && rank <= 100)
            {
                rankId = 3;
            }
            else if (rank > 100 && rank <= 200)
            {
                rankId = 2;
            }
            else if (rank > 200 && rank <= 400)
            {
                rankId = 1;
            }
            else if (rank > 400)
            {
                rankId = 0;
            }
            else
            {
                rankId = -1;
            }

            return rankId;
        }
    }
}