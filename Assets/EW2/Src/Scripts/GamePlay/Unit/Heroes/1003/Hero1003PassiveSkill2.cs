using UnityEngine;

namespace EW2
{
    public class Hero1003PassiveSkill2 : UnitSkill
    {
        public HeroData1003.PassiveSkill2[] passiveSkills;
        public HeroData1003.PassiveSkill2 CurrentPassiveData => passiveSkills[Level - 1];
        
        private float lastSkillTime;
        public Hero1003PassiveSkill2(Unit owner, HeroData1003.PassiveSkill2[] passiveSkills) : base(owner)
        {
            this.passiveSkills = passiveSkills;
        }

        public override void Init()
        {
            lastSkillTime = 0f;
        }

        public override void Execute()
        {
            lastSkillTime = Time.time;

        }

        public bool CanCastSkill()
        {
            if (Level > 0)
            {
                return Time.time - lastSkillTime >= 1f;
            }
            return false;
        }
        public float GetDamage()
        {
            var atk = owner.Stats.GetStat<Damage>(RPGStatType.Damage);

            return atk.StatValue;
        }
    }
}