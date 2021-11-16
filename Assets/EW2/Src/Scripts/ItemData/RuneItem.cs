using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class RuneItem : ItemInventoryBase
    {
        public int HeroIdEquip { get; set; }
        public int RuneIdConvert { get; set; }


        public RuneItem(int id, int quantity, int inventoryType, int level = 1) : base(id, quantity,
            inventoryType)
        {
            RuneIdConvert = id;
            this.Level = level;
            var runIdCompare = InventoryDataBase.GetRuneId(RuneIdConvert);
            this.Rarity = runIdCompare.Item2;
        }

        public bool IsLevelMax()
        {
            var levelMax = GameContainer.Instance.Get<InventoryDataBase>().GetRuneData(RuneIdConvert).runeStatBases
                .Length;
            return Level >= levelMax;
        }

        public bool IsEquipped()
        {
            return HeroIdEquip > 0;
        }

        public int GetMaxLevel()
        {
            var levelMax = GameContainer.Instance.Get<InventoryDataBase>().GetRuneData(RuneIdConvert).runeStatBases
                .Length;
            return levelMax;
        }

        public void EquipRune(int heroId)
        {
            HeroIdEquip = heroId;
        }

        public void UnequipRune()
        {
            HeroIdEquip = 0;
        }

        public Dictionary<RPGStatType, RPGStatModifier> AddStatRune(HeroStats hero)
        {
            Dictionary<RPGStatType, RPGStatModifier> listStatModifier = new Dictionary<RPGStatType, RPGStatModifier>();

            var dataCompare = InventoryDataBase.GetRuneId(RuneIdConvert);
            var dataRune = GameContainer.Instance.Get<InventoryDataBase>().GetRuneData(RuneIdConvert);
            var dataStat = dataRune.GetRuneStatBaseByLevel(Level);

            var statModifiable = Ultilities.GetStatModifiable(dataStat.statBonus);

            if (statModifiable != null)
            {
                if (dataCompare.Item1 == (int) RuneId.ImmortalRune)
                {
                    var statModifierHpRegen = new RPGStatModifier(statModifiable, ModifierType.TotalPercent,
                        dataStat.ratioBonus, false);
                    var statModifiableRevive = Ultilities.GetStatModifiable(dataStat.statApplyChance);
                    var statModifierRevive = new RPGStatModifier(statModifiableRevive, ModifierType.TotalAdd,
                        -dataStat.valueBonus, false);

                    hero.AddStatModifier(dataStat.statBonus, statModifierHpRegen);
                    listStatModifier.Add(dataStat.statBonus, statModifierHpRegen);
                    hero.AddStatModifier(dataStat.statApplyChance, statModifierRevive);
                    listStatModifier.Add(dataStat.statApplyChance, statModifierRevive);
                }
                else if (dataCompare.Item1 == (int) RuneId.ArgonyRune)
                {
                    // coming soon
                }
                else if (dataCompare.Item1 == (int) RuneId.MiseryRune)
                {
                    // coming soon
                }
                else if (dataCompare.Item1 == (int) RuneId.RageRune || dataCompare.Item1 == (int) RuneId.DeathRune ||
                         dataCompare.Item1 == (int) RuneId.LifeRune || dataCompare.Item1 == (int) RuneId.WisdomRune)
                {
                    var statModifier = new RPGStatModifier(statModifiable, ModifierType.TotalAdd,
                        dataStat.ratioBonus, false);

                    hero.AddStatModifier(dataStat.statBonus, statModifier);
                    listStatModifier.Add(dataStat.statBonus, statModifier);
                }
                else
                {
                    RPGStatModifier statModifier = null;

                    if (dataStat.ratioBonus > 0)
                    {
                        statModifier = new RPGStatModifier(statModifiable, ModifierType.TotalPercent,
                            dataStat.ratioBonus, false);
                    }
                    else if (dataStat.valueBonus > 0)
                    {
                        statModifier = new RPGStatModifier(statModifiable, ModifierType.TotalAdd,
                            dataStat.valueBonus, false);
                    }

                    if (statModifier != null)
                    {
                        hero.AddStatModifier(dataStat.statBonus, statModifier);
                        listStatModifier.Add(dataStat.statBonus, statModifier);
                    }
                }
            }

            return listStatModifier;
        }
    }
}