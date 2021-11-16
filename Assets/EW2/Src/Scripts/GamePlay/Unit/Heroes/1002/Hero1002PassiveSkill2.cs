using UnityEngine;

namespace EW2
{
    public class Hero1002PassiveSkill2 : UnitSkill
    {
        private readonly HeroData1002.PassiveSkill2[] passiveSkill;
        private readonly Hero1002 hero;
        private readonly RangerSearchTarget rangeSearchTarget;
        private HeroData1002.PassiveSkill2 dataPassiveCurr;

        public Hero1002PassiveSkill2(Unit owner, HeroData1002.PassiveSkill2[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
            hero = owner as Hero1002;
            rangeSearchTarget = hero.SearchTarget as RangerSearchTarget;
        }

        public override void Init()
        {
            if (rangeSearchTarget != null && Level > 0)
            {
                dataPassiveCurr = GetDataPassiveByLevel(Level);
                
                rangeSearchTarget.ScaleRangeDetect(1 + dataPassiveCurr.detectRangeBonus);
                // Debug.LogWarning("[Hero 1002] Skill passive 2 detect range scale: " + dataPassiveCurr.detectRangeBonus);
            }
        }

        public override void Execute()
        {
        }

        public override void Remove()
        {
        }

        private HeroData1002.PassiveSkill2 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < passiveSkill.Length)
                return passiveSkill[levelPassive - 1];
            return passiveSkill[passiveSkill.Length - 1];
        }
    }
}