using Spine;
using UnityEngine;

namespace EW2
{
    public class DummySpine : UnitSpine
    {
        public DummySpine(Unit owner) : base(owner)
        {
            appearName = "appear";

            idleName = "idle";

            moveName = "move";

            dieName = "die";

            stunName = idleName;

            attackMeleeName = "attack_melee_1";

            attackRangeName = "attack_range_1";
        }

        public override TrackEntry Idle()
        {
            if (owner.IsAlive == false)
                return null;

            //Debug.LogWarning(owner.name + idleName);
            return SetAnimation(0, idleName, true);
        }

        public override TrackEntry Appear()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, appearName, false);
        }

        public override TrackEntry Move()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, moveName, true);
        }

        public override TrackEntry Die()
        {
            return SetAnimation(0, dieName, false);
        }

        public override TrackEntry Stun()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, stunName, true);
        }

        public override TrackEntry AttackMelee()
        {
            if (owner.IsAlive == false)
                return null;

            var trackEntry = SetAnimation(0, attackMeleeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override TrackEntry AttackRange()
        {
            if (owner.IsAlive == false)
                return null;

            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }
    }
}