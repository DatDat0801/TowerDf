using Spine;

namespace EW2
{
    public class Reborn:UnitAction
    {
        public Reborn(Unit owner) : base(owner)
        {
        }

        protected override TrackEntry DoAnimation()
        {
            owner.UnitState.Set(ActionState.Reborn);

            return owner.UnitSpine.Reborn();
        }
    }
}