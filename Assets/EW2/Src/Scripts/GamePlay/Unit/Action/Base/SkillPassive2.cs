using Spine;

namespace EW2
{
    public class SkillPassive2 : UnitAction
    {
        public SkillPassive2(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.SkillPassive2);

            return owner.UnitSpine.SkillPassive2();
        }
    }
}