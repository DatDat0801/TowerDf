namespace EW2
{
    public class Hero1001PassiveSkill2 : UnitSkill
    {
        private HeroData1001.PassiveSkill2[] passiveSkill;

        public Hero1001PassiveSkill2(Unit owner, HeroData1001.PassiveSkill2[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
        }

        public HeroData1001.PassiveSkill2 SkillData => passiveSkill[Level -1];

        public override void Init()
        {
            if (Level > 0)
                ((Hero1001) owner).takeDamageCalculation = new Hero1001TakeDamageCalculation(owner, SkillData.reducePhysicDamage);
        }

        public override void Execute()
        {
        }

        public override void Remove()
        {
        }
    }
}