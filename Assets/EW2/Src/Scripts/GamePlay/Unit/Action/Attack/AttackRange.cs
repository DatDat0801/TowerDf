using Spine;

namespace EW2
{
    public class AttackRange : Attack
    {
        public AttackRange(Unit owner) : base(owner)
        {
            animationName = owner.UnitSpine.attackRangeName;

            Init();
        }

        public void SetTimeTriggerAtkSkill()
        {
            timeTriggerAttack = minTimeTriggerAttack;
            ResetTime();
        }

        protected override TrackEntry DoAnimation()
        {
            return owner.UnitSpine.AttackRange();
        }
    }
}