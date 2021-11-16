using Spine;
using Spine.Unity;

namespace EW2
{
    public class TotemSpine : UnitSpine
    {
        public TotemSpine(Unit owner) : base(owner)
        {
            this.dieName = "end";
            this.idleName = "idle";
        }
        public override TrackEntry Die()
        {
            return SetAnimation(0, dieName, false);
        }

        public override TrackEntry Idle()
        {
            return SetAnimation(0, this.idleName, false);
        }
    }
}