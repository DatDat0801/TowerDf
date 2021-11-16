using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EW2
{
    public class Hero1003PassiveSkill1 : UnitSkill
    {
        private float lastSkillTime;
        private Hero1003 hero;
        public HeroData1003.PassiveSkill1[] passiveSkill1s;
        public HeroData1003.PassiveSkill1 CurrentPassiveData => passiveSkill1s[Level - 1];

        public Hero1003PassiveSkill1(Unit owner, HeroData1003.PassiveSkill1[] passiveSkills) : base(owner)
        {
            this.passiveSkill1s = passiveSkills;
        }

        public override void Init()
        {
            lastSkillTime = 0f;
            hero = (Hero1003) owner;
            if (Level > 0)
                hero.passive1Collider.SetRadius(CurrentPassiveData.roarRadiusAffected);
        }

        public override void Execute()
        {
            lastSkillTime = Time.time;
            var hero1003 = (Hero1003) owner;
            var baseArmor = hero1003.Stats.GetStat<ArmorPhysical>(RPGStatType.Armor);
            //Debug.LogAssertion($"base physic armor before added: {baseArmor.StatValue}");
            baseArmor.AddModifier(GetBonusStat(baseArmor));
            // Debug.LogAssertion(
            //     $"*Added Value: {CurrentPassiveData.bonusArmor * baseArmor.StatBaseValue} current physic armor {baseArmor.StatValue}");
            //hero1003.UnitState.Set(ActionState.UseSkill);
            // UniTask.Delay(900);
            // hero1003.UnitState.Set(ActionState.UseSkill);
        }

        public RPGStatModifier GetBonusStat(RPGStatModifiable attribute)
        {
            var modifier = new RPGStatModifier(attribute, CurrentPassiveData.modifierType,
                CurrentPassiveData.bonusArmor * attribute.StatValue, false, owner);
            //Debug.LogAssertion("modify type: " + SkillData.modifierType + " modifier: " + modifier.Value);
            var action = new ModifierTimeRemoveAction(modifier, CurrentPassiveData.duration);

            action.Execute();

            modifier.AddAction(action);

            return modifier;
        }

        public bool CanCastSkill()
        {
            if (Level > 0)
            {
                return Time.time - lastSkillTime >= CurrentPassiveData.duration + CurrentPassiveData.cooldown;
            }

            return false;
        }
    }
}