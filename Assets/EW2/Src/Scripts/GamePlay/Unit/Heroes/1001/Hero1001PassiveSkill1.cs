namespace EW2
{
    public class Hero1001PassiveSkill1 : UnitSkill
    {
        private HeroData1001.PassiveSkill1[] passiveSkill;

        public Hero1001PassiveSkill1(Unit owner, HeroData1001.PassiveSkill1[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
        }

        public HeroData1001.PassiveSkill1 SkillData => passiveSkill[Level - 1];

        public float GetDamage()
        {
            var atk = owner.Stats.GetStat<Damage>(RPGStatType.Damage);

            return SkillData.damageBaseOnAtk * atk.StatValue;
        }

        public override void Init()
        {
            if (Level > 0)
                ((Hero1001) owner).SetAttackMelee(new Hero1001AttackMelee(owner, SkillData));
        }

        public override void Execute()
        {
        }

        public override void Remove()
        {
        }
    }
}