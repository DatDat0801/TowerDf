using Spine;
using Event = Spine.Event;

namespace EW2
{
    public class Soldier2003Spine : DummySpine
    {
        private Soldier2003 soldier2003;

        public Soldier2003Spine(Unit owner) : base(owner)
        {
            attackRangeName = "attack_range_1";
            passive1Name = "skill_1";
            passive2Name = "skill_2";
            soldier2003 = (Soldier2003) owner;
        }

        public override TrackEntry AttackRange()
        {
            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override TrackEntry SkillPassive1()
        {
            return SetAnimation(0, passive1Name, false);
        }

        public override TrackEntry SkillPassive2()
        {
            return SetAnimation(0, passive2Name, false);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("attack_range"))
            {
                soldier2003.SpawnBullet();
            }
            else if (e.Data.Name.Equals("skill_1"))
            {
                soldier2003.SpawnEffectSkill1();
            }
            else if (e.Data.Name.Equals("skill_2"))
            {
                soldier2003.SpawnEffectSkill2();
            }
        }
    }
}