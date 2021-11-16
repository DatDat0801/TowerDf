using Spine;

namespace EW2
{
    public class Crocodile6047Spine : DummySpine
    {
        public Crocodile6047Spine(Unit owner) : base(owner)
        {
            attackMeleeName = "bite";
        }

        public void Dive()
        {
            SetAnimation(0, "down_loop", true);
        }

        public TrackEntry Floating()
        {
            return SetAnimation(0, "up", false);
        }

        public override TrackEntry AttackMelee()
        {
            return SetAnimation(0, attackMeleeName, false);
        }
    }
}