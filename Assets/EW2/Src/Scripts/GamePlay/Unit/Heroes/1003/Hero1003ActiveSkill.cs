using UnityEngine;

namespace EW2
{
    public class Hero1003ActiveSkill : UnitSkill
    {
        private float lastSkillTime;
        private HeroData1003.ActiveSkill[] activeSkill;
        public HeroData1003.ActiveSkill SkillData => activeSkill[Level - 1];

        public Hero1003ActiveSkill(Unit owner, HeroData1003.ActiveSkill[] activeSkill) : base(owner)
        {
            this.activeSkill = activeSkill;
        }

        public override void Init()
        {
            lastSkillTime = 0f;
        }

        public override void Execute()
        {
            lastSkillTime = Time.time;
            var hero1003 = (Hero1003) owner;
            var health = hero1003.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            health.CurrentValue += SkillData.recoverHp * health.StatValue;

            //health.AddModifier(new RPGStatModifier(new HealthPoint(), SkillData.modifierType, SkillData.recoverHp * health.CurrentValue));
            // Debug.LogAssertion(
            //     $"Max value: {health.StatValue} health after added: %{SkillData.recoverHp * 100} NOW Health: {health.CurrentValue}");

            var baseDamage = hero1003.Stats.GetStat<Damage>(RPGStatType.Damage);

            //Debug.LogAssertion($"base damage before added: {baseDamage.StatValue}");

            baseDamage.AddModifier(GetBonusStat(baseDamage, health.StatValue));
            //Debug.LogAssertion(
            //    $"*Added Value: {SkillData.bonusNormalDamage * health.StatValue} current damage {baseDamage.StatValue}");
        }

        //For GD input, modifyType must be 2 TotalAdd
        public RPGStatModifier GetBonusStat(RPGStatModifiable attribute, float basedValue)
        {
            var modifier = new RPGStatModifier(attribute, SkillData.modifierType,
                SkillData.bonusNormalDamage * basedValue, false, owner);
            //Debug.LogAssertion("modify type: " + SkillData.modifierType + " modifier: " + modifier.Value);
            var action = new ModifierTimeRemoveAction(modifier, SkillData.duration);

            action.Execute();

            modifier.AddAction(action);

            return modifier;
        }

        public bool IsInDuration()
        {
            if (Time.time - lastSkillTime >= SkillData.duration && lastSkillTime > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsInAnimation()
        {
            //1.5f is the time that skill 1 play
            if (Time.time - lastSkillTime <= 1.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}