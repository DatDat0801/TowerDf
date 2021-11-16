using Spine;
using Event = Spine.Event;

namespace EW2
{
    public class Soldier2001Spine : DummySpine
    {
        private Soldier2001 soldier2001;

        public Soldier2001Spine(Unit owner) : base(owner)
        {
            attackRangeName = "attack_range_1";
            soldier2001 = (Soldier2001) owner;
        }

        public override TrackEntry AttackRange()
        {
            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("attack_range"))
            {
                soldier2001.SpawnBullet();
            }
        }
    }
}