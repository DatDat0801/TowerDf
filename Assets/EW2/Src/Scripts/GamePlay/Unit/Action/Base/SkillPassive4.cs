using Spine;

namespace EW2
{
    public class SkillPassive4 : UnitAction
    {
        public SkillPassive4(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.SkillPassive4);

            return owner.UnitSpine.SkillPassive4();
        }
    }
}