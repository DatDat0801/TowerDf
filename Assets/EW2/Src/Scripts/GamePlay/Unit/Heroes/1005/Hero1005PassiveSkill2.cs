namespace EW2
{
    public class Hero1005PassiveSkill2: UnitSkill
    {
        private HeroData1005.PassiveSkill2[] _passiveSkill2s;
        public Hero1005PassiveSkill2(Unit owner, HeroData1005.PassiveSkill2[] passiveSkill) : base(owner)
        {
            this._passiveSkill2s = passiveSkill;
        }

        public override void Init()
        {
            
        }

        public override void Execute()
        {
            
        }
        public HeroData1005.PassiveSkill2 SkillData => this._passiveSkill2s[Level - 1];
    }
}