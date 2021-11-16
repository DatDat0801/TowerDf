using Hellmade.Sound;
using Spine;

namespace EW2
{
    public class Pet1004Spine : DummySpine
    {
        private Pet1004 control;

        private string turnName = "turn";
        private string idleRelax = "idle_relax";

        public Pet1004Spine(Unit owner) : base(owner)
        {
            control = owner as Pet1004;
            attackRangeName = "attack_range_1";
            attackMeleeName = "attack_melee_1";
            activeSkillName = "skill_active_1";
            passive3Name = "skill_passive_3";
            dieName = "move_die";
        }

        public override TrackEntry Turn()
        {
            return SetAnimation(0, turnName, false);
        }

        public TrackEntry IdleRelax()
        {
            return SetAnimation(0, idleRelax, false);
        }

        public override TrackEntry AttackRange()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_basic_attack_range_throw");
            EazySoundManager.PlaySound(audioClip);
            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }
        
        public override TrackEntry SkillAttack()
        {
            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_active_skill_throw");
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, activeSkillName, false);
        }

        public override TrackEntry SkillPassive3()
        {
            return SetAnimation(0, passive3Name, false);
        }

        public override TrackEntry Die()
        {
            return SetAnimation(0, dieName, true);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("attack_range"))
            {
                if (control != null)
                {
                    control.SpawnBulletRangeAttack();
                }
            }
            else if (e.Data.Name.Equals("skill_active_1"))
            {
                if (control != null)
                {
                    control.SpawnBulletActive();
                }
            }
            else if (e.Data.Name.Equals("skill_passive_3"))
            {
                if (control != null)
                {
                    control.SpawnBulletPassive3();
                }
            }
            else if (e.Data.Name.Equals("turn"))
            {
                if (control != null)
                {
                    control.DoFlip();
                }
            }
            else if (e.Data.Name.Equals("attack_melee"))
            {
                if (control != null)
                {
                    var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_basic_attack_melee");
                    EazySoundManager.PlaySound(audioClip);
                    control.SpawnEffectMeleeAttack();
                    control.normalAttackBox.Trigger();
                }
            }
        }
    }
}