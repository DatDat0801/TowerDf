using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace EW2
{
    [Serializable]
    public class RuneSetItem
    {
        public int Id { get; set; }
        public int RuneSet { get; set; }

        public RuneSetItem(int id, int typeSet)
        {
            Id = id;
            RuneSet = typeSet;
        }


        public Dictionary<RPGStatType, RPGStatModifier> AddStatRune(HeroStats hero)
        {
            Dictionary<RPGStatType, RPGStatModifier> listStatModifier = new Dictionary<RPGStatType, RPGStatModifier>();

            var dataRuneSet = GameContainer.Instance.Get<InventoryDataBase>().Get<RuneSetBonusDatabase>()
                .GetDataRuneSet(Id, RuneSet);

            var statModifiable = Ultilities.GetStatModifiable(dataRuneSet.statType[0]);

            if (statModifiable != null)
            {
                if (Id == (int) RuneId.ImmortalRune)
                {
                    var statModifierHpRegen = new RPGStatModifier(statModifiable, ModifierType.TotalPercent,
                        dataRuneSet.statValue[0], true);
                    var statModifiableRevive = Ultilities.GetStatModifiable(dataRuneSet.statType[1]);
                    var statModifierRevive = new RPGStatModifier(statModifiableRevive, ModifierType.TotalAdd,
                        -dataRuneSet.statValue[1], true);

                    hero.AddStatModifier(dataRuneSet.statType[0], statModifierHpRegen);
                    listStatModifier.Add(dataRuneSet.statType[0], statModifierHpRegen);
                    hero.AddStatModifier(dataRuneSet.statType[1], statModifierRevive);
                    listStatModifier.Add(dataRuneSet.statType[1], statModifierRevive);
                }
                else if (Id == (int) RuneId.LifeRune)
                {
                    if (hero.GetStat(dataRuneSet.statType[0]).StatValue <= 0)
                    {
                        var statModifier = new RPGStatModifier(statModifiable, ModifierType.TotalPercent,
                            dataRuneSet.statValue[0], false);

                        hero.AddStatModifier(dataRuneSet.statType[0], statModifier);
                        listStatModifier.Add(dataRuneSet.statType[0], statModifier);
                    }
                    else
                    {
                        var statModifier = new RPGStatModifier(statModifiable, ModifierType.BasePercent,
                            dataRuneSet.statValue[0], false);

                        hero.AddStatModifier(dataRuneSet.statType[0], statModifier);
                        listStatModifier.Add(dataRuneSet.statType[0], statModifier);
                    }
                }
                else if (Id == (int) RuneId.WisdomRune || Id == (int) RuneId.RageRune || Id == (int) RuneId.DeathRune )//move RangeRune, Death from above 
                {
                    var statModifier = new RPGStatModifier(statModifiable, ModifierType.TotalAdd,
                        dataRuneSet.statValue[0], false);
                    // Debug.LogAssertion(
                    //     $"id rune {Id} before add crit chance {hero.GetStat(RPGStatType.CritChance).StatValue}");
                    hero.AddStatModifier(dataRuneSet.statType[0], statModifier);
                    // Debug.LogAssertion(
                    //     $"id rune {Id} AFTER add crit chance {hero.GetStat(RPGStatType.CritChance).StatValue}");
                    listStatModifier.Add(dataRuneSet.statType[0], statModifier);
                }
                else if (Id == (int) RuneId.ArgonyRune)
                {
                    // coming soon
                }
                else if (Id == (int) RuneId.MiseryRune)
                {
                    // coming soon
                }
                else
                {
                    RPGStatModifier statModifier = statModifier = new RPGStatModifier(statModifiable,
                        ModifierType.TotalPercent, dataRuneSet.statValue[0], true);

                    if (statModifier != null)
                    {
                        hero.AddStatModifier(dataRuneSet.statType[0], statModifier);
                        listStatModifier.Add(dataRuneSet.statType[0], statModifier);
                    }
                }
            }

            return listStatModifier;
        }
    }
}