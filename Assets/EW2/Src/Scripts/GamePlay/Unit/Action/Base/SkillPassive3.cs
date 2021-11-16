using Spine;

namespace EW2
{
    public class SkillPassive3 : UnitAction
    {
        public SkillPassive3(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.SkillPassive3);
            
            return owner.UnitSpine.SkillPassive3();
        }
    }
}