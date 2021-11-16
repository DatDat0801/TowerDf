using System;
using UnityEngine;

namespace EW2
{
    [Serializable]
    public class RuneStatBase
    {
        public int level;
        
        public float ratioBonus;
        
        public float valueBonus;

        public RPGStatType statBonus;

        // apply to rune special
        public RPGStatType statApplyChance;
    }

    [Serializable]
    public class RuneDataBase
    {
        public int runeId;
        public int rarity;
        public RuneStatBase[] runeStatBases;

        public RuneStatBase GetRuneStatBaseByLevel(int level)
        {
            if (level < runeStatBases.Length)
                return runeStatBases[level - 1];

            return runeStatBases[runeStatBases.Length - 1];
        }
    }

    public class RuneDataBases : ScriptableObject
    {
        public RuneDataBase[] runeDataBases;
    }
}