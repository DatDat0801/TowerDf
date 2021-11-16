namespace EW2
{
    public class TakeDamageCalculation
    {
        protected Unit owner;

        public TakeDamageCalculation(Unit owner)
        {
            this.owner = owner;
        }
        
        public virtual float Calculate(DamageInfo info)
        {
            float armor = 0;
            switch (info.damageType)
            {
                case DamageType.Magical:
                    armor = owner.Stats.GetStat<RPGAttribute>(RPGStatType.Resistance).StatValue;
                    break;
                case DamageType.Physical:
                    armor = owner.Stats.GetStat<RPGAttribute>(RPGStatType.Armor).StatValue;
                    break;
                case DamageType.True:
                    armor = 0;
                    break;
            }
            
            return info.value * (100 / (100 + armor));
        }
    }
}