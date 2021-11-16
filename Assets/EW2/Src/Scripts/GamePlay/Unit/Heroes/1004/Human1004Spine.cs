using Hellmade.Sound;
using Spine;

namespace EW2
{
    public class Human1004Spine : DummySpine
    {
        private Human1004 control;

        private string turnName = "turn";
        private string idleRelax = "idle_relax";
        private string attackRangeName2 = "attack_range_2";
        private string dieLoopName = "die_loop";
        private string dieToIdle = "die_to_idle";

        public Human1004Spine(Unit owner) : base(owner)
        {
            control = owner as Human1004;
            attackRangeName = "attack_range_1";
            activeSkillName = "skill_active_1";
            passive2Name = "skill_passive_2";
        }

        public TrackEntry DieLoop()
        {
            return SetAnimation(0, dieLoopName, true);
        }

        public TrackEntry DieToIdle()
        {
            return SetAnimation(0, dieToIdle, false);
        }

        public override TrackEntry Turn()
        {
            return SetAnimation(0, turnName, false);
        }

        public TrackEntry IdleRelax()
        {
            return SetAnimation(0, idleRelax, false);
        }


        int count = 0;

        public override TrackEntry AttackRange()
        {
            TrackEntry trackEntry = null;

            if (count == 0)
            {
                count = 1;
                trackEntry = SetAnimation(0, attackRangeName, false);
            }
            else
            {
                count = 0;
                trackEntry = SetAnimation(0, attackRangeName2, false);
            }

            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override TrackEntry SkillAttack()
        {
            return SetAnimation(0, activeSkillName, false);
        }

        public override TrackEntry SkillPassive2()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_passive_2_throw");
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, passive2Name, false);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("skill_passive_2"))
            {
                if (control != null)
                {
                    control.SpawnBulletPassive();
                }
            }
            else if (e.Data.Name.Equals("turn"))
            {
                if (control != null)
                {
                    control.DoFlip();
                }
            }
        }
    }
}