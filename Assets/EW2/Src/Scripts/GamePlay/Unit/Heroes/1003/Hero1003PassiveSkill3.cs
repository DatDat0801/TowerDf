namespace EW2
{
    public class Hero1003PassiveSkill3:UnitSkill
    {
        public HeroData1003.PassiveSkill3[] passiveSkills;
        public HeroData1003.PassiveSkill3 CurrentPassiveData => passiveSkills[Level - 1];
        public Hero1003PassiveSkill3(Unit owner, HeroData1003.PassiveSkill3[] passiveSkills) : base(owner)
        {
            this.passiveSkills = passiveSkills;
        }

        public override void Init()
        {
           
        }

        public override void Execute()
        {
            
        }
    }
}