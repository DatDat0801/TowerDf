using Spine;

namespace EW2
{
    public class Enemy3013Spine: DummySpine
    {
        public Enemy3013Spine(Unit owner) : base(owner)
        {
            this.moveName = "move";
        }

        public override TrackEntry Die()
        {
            return null;
        }
    }
}