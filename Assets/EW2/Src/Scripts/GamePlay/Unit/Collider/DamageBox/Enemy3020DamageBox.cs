namespace EW2
{
    public class Enemy3020DamageBox : EnemyNormalDamageBox
    {
        private float damageRate = 1f;

        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = base.GetDamage(target);

            var dataDamageRate = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>()
                .GetDamageRate(owner.Level);

            if (((Enemy3020) owner).CurrentPhase == Phase3020.Phase2)
                damageRate = dataDamageRate.damageRateMeleePhase2;
            else
                damageRate = dataDamageRate.damageRateMeleePhase3;

            damageInfo.value *= damageRate;

            return damageInfo;
        }
    }
}