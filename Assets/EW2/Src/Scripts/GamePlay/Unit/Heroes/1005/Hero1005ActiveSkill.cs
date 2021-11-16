namespace EW2
{
    public class Hero1005ActiveSkill: UnitSkill
    {
        private HeroData1005.ActiveSkill[] _activeSkills;
        public Hero1005ActiveSkill(Unit owner,  HeroData1005.ActiveSkill[] activeSkills) : base(owner)
        {
            this._activeSkills = activeSkills;
        }

        public override void Init()
        {
            
        }

        public override void Execute()
        {
            
        }
        public HeroData1005.ActiveSkill SkillData => this._activeSkills[Level - 1];
    }
}