using Spine;

namespace EW2
{
    public class GetHurt : UnitAction
    {
        public GetHurt(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.GetHurt);
            
            return owner.UnitSpine.GetHurt();
        }
    }
}