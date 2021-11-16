using Spine;
using UnityEngine;

namespace EW2
{
    public class Hero1001AttackMelee : AttackMelee
    {
        private int countNumberAttack;

        private HeroData1001.PassiveSkill1 skillData;

        public Hero1001AttackMelee(Unit owner, HeroData1001.PassiveSkill1 skillData, float range = 0.6f) : base(owner,
            range)
        {
            this.skillData = skillData;
        }

        protected override TrackEntry DoAnimation()
        {
            countNumberAttack++;
                
            //Debug.Log("count attack: " + countNumberAttack);

            if (countNumberAttack == skillData.basicAttackCountEffect)
            {
                countNumberAttack = 0;

                return owner.UnitSpine.SkillPassive1();
            }

            return owner.UnitSpine.AttackMelee();
        }
    }
}