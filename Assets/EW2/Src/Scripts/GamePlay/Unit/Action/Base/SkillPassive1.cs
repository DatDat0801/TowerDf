using Spine;

namespace EW2
{
    public class SkillPassive1 : UnitAction
    {
        public SkillPassive1(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.SkillPassive1);
            
            return owner.UnitSpine.SkillPassive1();
        }
    }
}