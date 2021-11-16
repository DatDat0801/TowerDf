namespace EW2
{
    public class Enemy3020Skill4Impact : NormalDamageBox<EnemyBase>
    {
        private EnemyData3020.EnemyData3020Skill4 skillData;
        private DamageInfo damageInfo;

        public void InitImpactSkill(Enemy3020 enemy3020, EnemyData3020.EnemyData3020Skill4 data)
        {
            this.owner = enemy3020;
            this.skillData = data;
            damageInfo = new DamageInfo()
            {
                creator = owner,
                damageType = skillData.damageType,
                value = skillData.damage
            };
            Trigger(0, 1.4f);
        }

        public StunStatus GetStun(Unit target)
        {
            return new StunStatus(new StatusOverTimeConfig()
            {
                creator = owner,
                owner = target,
                lifeTime = skillData.timeStun
            });
        }

        protected override bool CanGetDamage(Unit target)
        {
            return isAoe;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            target.StatusController.AddStatus(GetStun(target));
            return this.damageInfo;
        }
    }
}