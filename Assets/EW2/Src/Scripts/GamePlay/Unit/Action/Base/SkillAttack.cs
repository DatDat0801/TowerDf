using Spine;

namespace EW2
{
    public class SkillAttack : UnitAction
    {
        public SkillAttack(Unit owner) : base(owner)
        {
            
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.UseSkill);
            
            return owner.UnitSpine.SkillAttack();
        }
    }
}