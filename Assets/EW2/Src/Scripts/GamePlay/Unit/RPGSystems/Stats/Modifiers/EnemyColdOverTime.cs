namespace EW2
{
    public class EnemyColdOverTime : ColdStatusOverTime
    {
        public EnemyColdOverTime(StatusOverTimeConfig config, float slowDownPercent) : base(config, slowDownPercent)
        {
            statusType = StatusType.Cold;

            Stacks = false;
            if (this.config.owner is EnemyBase @base)
            {
                this.target = @base;
            }
            this.slowDownPercent = slowDownPercent;
        }
    }
}