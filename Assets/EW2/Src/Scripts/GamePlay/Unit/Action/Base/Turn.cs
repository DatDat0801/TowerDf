using Spine;

namespace EW2
{
    public class Turn : UnitAction
    {
        public Turn(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.Turn);
            return owner.UnitSpine.Turn();
        }
    }
}