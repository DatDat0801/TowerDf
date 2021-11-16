namespace EW2
{
    public class StatusOverTimeConfig
    {
        public Unit creator;
        public Unit owner;
        public float lifeTime;
        public float intervalTime;
        public float delayTime;
        public float baseValue;
        public float chanceApply = 1f;
        public DamageType damageType;
        public StatusType statusType = StatusType.ModifierStatOverTime;
    }
}