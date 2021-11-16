namespace EW2
{
    public class Hero1005PassiveSkill1: UnitSkill
    {
        private HeroData1005.PassiveSkill1[] _passiveSkill1s;
        public Hero1005PassiveSkill1(Unit owner, HeroData1005.PassiveSkill1[] passiveSkill) : base(owner)
        {
            this._passiveSkill1s = passiveSkill;
        }

        public override void Init()
        {
            
        }

        public override void Execute()
        {
            
        }
        public HeroData1005.PassiveSkill1 SkillData => this._passiveSkill1s[Level - 1];
    }
}