using UnityEngine;

namespace EW2
{
    public class Hero1004PassiveSkill2 : UnitSkill
    {
        private readonly HeroData1004.PassiveSkill2[] passiveSkill;
        private readonly Pet1004 hero;
        public HeroData1004.PassiveSkill2 dataPassiveCurr;
        private bool activePassive;

        public Hero1004PassiveSkill2(Unit owner, HeroData1004.PassiveSkill2[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
            hero = owner as Pet1004;
        }

        public override void Init()
        {
            if (Level > 0)
            {
                dataPassiveCurr = GetDataPassiveByLevel(Level);
            }
        }

        public override void Execute()
        {
            if (activePassive)
                activePassive = false;
        }

        public override void Remove()
        {
        }

        public bool IsActive()
        {
            return activePassive;
        }

        public void CheckActivePassive()
        {
            if (activePassive) return;

            var rand = Random.Range(0f, 1f);
            if (rand <= dataPassiveCurr.chance)
                activePassive = true;
        }

        private HeroData1004.PassiveSkill2 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < passiveSkill.Length)
                return passiveSkill[levelPassive - 1];
            return passiveSkill[passiveSkill.Length - 1];
        }
    }
}