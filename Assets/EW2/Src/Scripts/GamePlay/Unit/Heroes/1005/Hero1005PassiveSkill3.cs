namespace EW2
{
    public class Hero1005PassiveSkill3: UnitSkill
    {
        private readonly HeroData1005.PassiveSkill3[] _passiveSkill3s;
        public Hero1005PassiveSkill3(Unit owner , HeroData1005.PassiveSkill3[] passiveSkill) : base(owner)
        {
            this._passiveSkill3s = passiveSkill;
        }

        public override void Init()
        {
            
        }

        public override void Execute()
        {
            
        }
        public HeroData1005.PassiveSkill3 SkillData => this._passiveSkill3s[Level - 1];
    }
}