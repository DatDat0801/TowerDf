using Hellmade.Sound;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class Hero1001PassiveSkill3 : UnitSkill, IUpdateSystem
    {
        private HeroData1001.PassiveSkill3[] passiveSkill;

        public HeroData1001.PassiveSkill3 SkillData => passiveSkill[Level - 1];

        public Hero1001PassiveSkill3(Unit owner, HeroData1001.PassiveSkill3[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
        }

        public override void Init()
        {
        }

        public override void Execute()
        {
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public override void Remove()
        {
            Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        public ShieldPoint GetShield(Unit target)
        {
            return new Hero1001ShieldPoint(new StatusOverTimeConfig()
            {
                creator = owner,
                owner = target,
                lifeTime = SkillData.time,
                baseValue = SkillData.valueShield
            });
        }

        public RPGStatModifier GetBonusStat(RPGStatModifiable attribute, Unit target)
        {
            var modifier = new RPGStatModifier(attribute, SkillData.modifierType, SkillData.bonusPhysicArmor, false,
                owner, target);

            var action = new ModifierTimeRemoveAction(modifier, SkillData.time);

            action.Execute();

            modifier.AddAction(action);

            return modifier;
        }

        private float countTime = 0;

        public void OnUpdate(float deltaTime)
        {
            if (owner.IsAlive && Level > 0)
            {
                countTime += deltaTime;

                if (countTime >= SkillData.cooldown && ((HeroBase) owner).CanControl &&
                    owner.UnitSpine.IsPlayingAnimation(owner.UnitSpine.idleName))
                {
                    var hero = (Hero1001) owner;

                    hero.UseSkillPassive3();

                    hero.passiveSkill3.Trigger();

                    countTime = 0;
                }
            }
        }
    }
}