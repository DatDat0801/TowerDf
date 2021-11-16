namespace EW2
{
    public class Hero1002ActiveSkill : UnitSkill
    {
        private HeroData1002.ActiveSkill[] activeSkill;

        public Hero1002ActiveSkill(Unit owner, HeroData1002.ActiveSkill[] activeSkill) : base(owner)
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

        public HeroData1002.ActiveSkill SkillData => activeSkill[Level - 1];
    }
}