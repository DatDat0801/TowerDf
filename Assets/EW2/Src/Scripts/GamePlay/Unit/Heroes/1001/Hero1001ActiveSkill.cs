namespace EW2
{
    public class Hero1001ActiveSkill : UnitSkill
    {
        private HeroData1001.ActiveSkill[] activeSkill;

        public Hero1001ActiveSkill(Unit owner, HeroData1001.ActiveSkill[] activeSkill) : base(owner)
        {
            this.activeSkill = activeSkill;
        }

        public override void Init()
        {
        }

        public override void Execute()
        {
        }

        public HeroData1001.ActiveSkill SkillData => activeSkill[Level - 1];

        public float GetDamage()
        {
            //var atk = owner.Stats.GetStat<Damage>(RPGStatType.Damage);

            return SkillData.baseDamage; //+ SkillData.damageBaseOnAtk * atk.StatValue;
        }

        public StunStatus GetStun(Unit target)
        {
            // return new StunStatus(owner, target, SkillData.lifeTime);
            return new StunStatus(new StatusOverTimeConfig()
            {
                creator = owner,
                owner = target,
                lifeTime = SkillData.lifeTime
            });
        }
    }
}