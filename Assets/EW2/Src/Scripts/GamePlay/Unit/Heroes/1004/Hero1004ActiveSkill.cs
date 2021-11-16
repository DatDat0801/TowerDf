namespace EW2
{
    public class Hero1004ActiveSkill : UnitSkill
    {
        private HeroData1004.ActiveSkill[] activeSkill;

        public Hero1004ActiveSkill(Unit owner, HeroData1004.ActiveSkill[] activeSkill) : base(owner)
        {
            this.activeSkill = activeSkill;
        }

        public override void Init()
        {
        }

        public override void Execute()
        {
        }

        public override void Remove()
        {
        }

        public HeroData1004.ActiveSkill SkillData => activeSkill[Level - 1];
    }
}