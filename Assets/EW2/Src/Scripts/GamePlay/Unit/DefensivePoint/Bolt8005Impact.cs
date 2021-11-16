namespace EW2
{
    public class Bolt8005Impact : DamageBox<Unit>
    {
        private float _damageValue;

        public void Initialize(float damage)
        {
            this.isAoe = false;
            this._damageValue = damage;
        }

        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo() {
                creator = this.owner,
                target = target,
                damageType = this.owner.DamageType,
                showVfxNormalAtk = false,
                value = this._damageValue
            };
            return damageInfo;
        }
    }
}