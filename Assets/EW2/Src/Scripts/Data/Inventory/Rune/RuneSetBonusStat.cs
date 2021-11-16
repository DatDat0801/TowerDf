using System;

namespace EW2
{
    [Serializable]
    public class RuneSetBonusStat
    {
        public int runeId;
        public RPGStatType[] statType;
        public float[] statValue;
        public int setQuantity;
    }
}