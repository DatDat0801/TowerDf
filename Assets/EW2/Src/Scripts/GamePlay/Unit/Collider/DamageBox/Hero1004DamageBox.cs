namespace EW2
{
    public class Hero1004DamageBox : HeroNormalDamageBox
    {
        private float timeLifePoisonStatus;
        private float damagePoisonStatus;

        public Hero1004DamageBox()
        {
            var hero1004 = (Pet1004) owner;
            if (hero1004)
            {
                var poisonStatusData = ((HeroData1004) hero1004.HeroData).poisonStatuses[hero1004.Level - 1];
                timeLifePoisonStatus = poisonStatusData.timeLife;
                damagePoisonStatus = poisonStatusData.hpPerSecond;
            }
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            //poison
            var poison = new PoisonStatus(new StatusOverTimeConfig()
            {
                creator = this.owner,
                owner = target,
                lifeTime = timeLifePoisonStatus,
                intervalTime = 1,
                baseValue = damagePoisonStatus,
                damageType = owner.DamageType,
                statusType = StatusType.Poison
            });
            poison.Stacks = false;

            target.StatusController.AddStatus(poison);

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                target = target
            };

            (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);
            return damageInfo;
        }
    }
}