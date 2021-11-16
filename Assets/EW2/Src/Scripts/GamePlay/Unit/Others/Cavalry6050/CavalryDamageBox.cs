namespace EW2
{
    public class CavalryDamageBox : DamageBox<GroupCavalryController>
    {
        private GetDamageCalculation damageCalculation;

        protected void Start()
        {
            damageCalculation = new GetDamageCalculation(owner);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType
            };

            (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);

            target.StatusController.AddStatus(GetStun(target));

            return damageInfo;
        }


        public StunStatus GetStun(Unit target)
        {
            return new StunStatus(new StatusOverTimeConfig()
            {
                creator = owner,
                owner = target,
                lifeTime = owner.DataCavalry.timeStun
            });
        }
    }
}