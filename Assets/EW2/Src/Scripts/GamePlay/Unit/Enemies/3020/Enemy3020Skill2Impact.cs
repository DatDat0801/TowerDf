namespace EW2
{
    public class Enemy3020Skill2Impact : DamageBox<Enemy3020>
    {
        private EnemyData3020.EnemyData3020Skill2 dataImpact;

        public void InitImpact(Enemy3020 enemy3020, EnemyData3020.EnemyData3020Skill2 dataSkill)
        {
            owner = enemy3020;
            dataImpact = dataSkill;
            Trigger(0, 1.85f);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return target.IsAlive;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (!CanGetDamage(target))
            {
                return null;
            }

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = dataImpact.damageType,

                value = dataImpact.damage,

                showVfxNormalAtk = true
            };

            return damageInfo;
        }
    }
}