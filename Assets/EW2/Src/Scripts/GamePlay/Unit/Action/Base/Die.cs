using Spine;

namespace EW2
{
    public class Die : UnitAction
    {
        public Die(Unit owner) : base(owner)
        {
            
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.Die);
            
            return owner.UnitSpine.Die();
        }
    }
}