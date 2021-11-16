namespace EW2
{
    public class Hero1004PassiveSkill1 : UnitSkill
    {
        private HeroData1004.PassiveSkill1[] passiveSkill;
        private bool activePassive;
        private HeroData1004.PassiveSkill1 dataPassiveCurr;

        public Hero1004PassiveSkill1(Unit owner, HeroData1004.PassiveSkill1[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
        }

        public override void Init()
        {
            activePassive = Level > 0;
            if (Level > 0)
                dataPassiveCurr = GetDataPassiveByLevel(Level);
        }

        public override void Execute()
        {
        }

        public override void Remove()
        {
        }

        private HeroData1004.PassiveSkill1 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < passiveSkill.Length)
                return passiveSkill[levelPassive - 1];
            return passiveSkill[passiveSkill.Length - 1];
        }

        public bool IsActive()
        {
            return activePassive;
        }

        public HeroData1004.PassiveSkill1 GetDataPassive()
        {
            return dataPassiveCurr;
        }
    }
}