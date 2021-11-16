using Spine;

namespace EW2
{
    public class Appear: UnitAction
    {
        public Appear(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.Appear);
            
            return owner.UnitSpine.Appear();
        }
    }
}