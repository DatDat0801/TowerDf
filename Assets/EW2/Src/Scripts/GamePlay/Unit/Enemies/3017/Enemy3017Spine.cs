using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3017Spine : DummySpine
    {
        private Enemy3017 enemy;
        private string appearAnimationName;
        private string attackMeleeName1;
        private TrackEntry appearTrackEntry;

        public Enemy3017Spine(Unit owner) : base(owner)
        {
            enemy = (Enemy3017) owner;
            attackMeleeName1 = "attack_melee_1";
            appearAnimationName = "appear";
        }

        public void SetAnimationAppearAndStop()
        {
            AnimationState.ClearTracks();
            appearTrackEntry = SetAnimation(0, appearAnimationName, false);
            appearTrackEntry.TimeScale = 0;
        }

        public void ResumeAppearAnimation()
        {
            appearTrackEntry.TimeScale = 1;
            AddAnimation(0, idleName, true, 0);
        }

        public override TrackEntry AttackMelee()
        {
            var trackEntry = SetAnimation(0, attackMeleeName1, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_melee":
                    if (trackEntry.Animation.Name.Equals(attackMeleeName1))
                    {
                        enemy.normalAttackBox1.Trigger();
                    }

                    break;
            }
        }

        protected override void EndAnimation(TrackEntry trackEntry)
        {
            var animationName = trackEntry.Animation.Name;
            if (animationName == appearAnimationName)
            {
                enemy.StartPhase1();
            }
        }


        public override TrackEntry Idle()
        {
            if (CalculateCurrentAnimationName(0) == idleName)
            {
                return default;
            }

            return SetAnimation(0, idleName, true);
        }
    }
}