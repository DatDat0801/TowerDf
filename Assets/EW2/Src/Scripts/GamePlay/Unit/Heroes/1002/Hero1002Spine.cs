using Hellmade.Sound;
using Spine;
using Event = Spine.Event;

namespace EW2
{
    public class Hero1002Spine : DummySpine
    {
        private Hero1002 control;

        public Hero1002Spine(Unit owner) : base(owner)
        {
            control = owner as Hero1002;
            attackRangeName = "attack_range_1";
            activeSkillName = "skill_active_1";
            passive3Name = "skill_passive_3";
        }

        public override TrackEntry AttackRange()
        {
            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override TrackEntry SkillAttack()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1002/sfx_hero_arryn_active_shoot_1");
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, activeSkillName, false);
        }

        public override TrackEntry SkillPassive3()
        {
            return SetAnimation(0, this.passive3Name, false);
        }

        public override TrackEntry Die()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.Hero1002Die());
            EazySoundManager.PlaySound(audioClip);
            return base.Die();
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("attack_range_1"))
            {
                if (control != null)
                {
                    control.SpawnBullet();

                    var audioClip1 = ResourceUtils.LoadSound(SoundConstant.Hero1002NormalAttack());
                    EazySoundManager.PlaySound(audioClip1);
                }
            }
            else if (e.Data.Name.Equals("skill_active_1") || e.Data.Name.Equals("skill_passive_3"))
            {
                if (control != null)
                {
                    control.SpawnBullet();
                }
            }
            else if (e.Data.Name.Equals("attack_melee"))
            {
                if (control != null)
                {
                    control.normalAttackBox.Trigger();

                    var audioClip1 = ResourceUtils.LoadSound(SoundConstant.Hero1002NormalAttack());
                    EazySoundManager.PlaySound(audioClip1);
                }
            }
        }
    }
}