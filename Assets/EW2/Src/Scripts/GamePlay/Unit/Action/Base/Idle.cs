using Spine;

namespace EW2
{
    public class Idle : UnitAction
    {
        public Idle(Unit owner) : base(owner)
        {
            
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.Idle);
            
            return owner.UnitSpine.Idle();
        }
    }
}