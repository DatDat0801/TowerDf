using Spine;

namespace EW2
{
    public class Ship6045Spine: DummySpine
    {
        public Ship6045Spine(Unit owner) : base(owner)
        {
            appearName = "open_door";
            
            idleName = "water";

            attackRangeName = "shoot_canon";
        }
        
        public override TrackEntry Appear()
        {
            return SetAnimation(1, appearName, false);
        }
        
        public override TrackEntry AttackRange()
        {
            return SetAnimation(1, attackRangeName, false);
        }
    }
}