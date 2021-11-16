using Hellmade.Sound;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Hero1005Spine : DummySpine
    {
        private readonly Hero1005 _control;
        private string _lastAttackRange = "attack_range_1";
        private readonly string _attackRange1;
        private readonly string _dieLoop = "die_loop";

        public Hero1005Spine(Unit owner) : base(owner)
        {
            this._control = owner as Hero1005;
            attackRangeName = "attack_range_1";
            activeSkillName = "skill_active_1";
            this._attackRange1 = "attack_range_2";
            this.rebornName = "die_to_idle";
        }

        public override TrackEntry AttackRange()
        {
            var animationName = this._lastAttackRange == this.attackRangeName
                ? this._attackRange1
                : this.attackRangeName;
            _lastAttackRange = animationName;
            var entry = SetAnimation(0, animationName, false);
            //var entry = SetAnimation(0, attackMeleeName, false);
            entry.MixDuration = 0.1f;
            entry.MixTime = 0.1f;
            //return entry;
            //var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref entry);
        }

        public override TrackEntry SkillAttack()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1005_ACTIVE_SKILL_CHARGE);
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, activeSkillName, false);
        }

        public override TrackEntry Die()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1005_DIE);
            EazySoundManager.PlaySound(audioClip);
            return base.Die();
        }

        public TrackEntry DieLoop()
        {
            return SetAnimation(0, this._dieLoop, true);
        }

        public override TrackEntry Reborn()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1005_REVIVE);
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, this.rebornName, false);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            if (e.Data.Name.Equals("attack_range"))
            {
                if (this._control != null)
                {
                    this._control.SpawnBullet();

                    if (this._control.IsInPassive2())
                    {
                        var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1005_PASSIVE2);
                        EazySoundManager.PlaySound(audioClip1);
                    }
                }
            }
            else if (e.Data.Name.Equals("skill_active_1"))
            {
                if (this._control != null)
                {
                    _control.SpawnBullet();
                }
            }
            else if (e.Data.Name.Equals("attack_melee"))
            {
                if (this._control != null)
                {
                    _control.normalAttackBox.Trigger();

                    var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1005_MELEE);
                    EazySoundManager.PlaySound(audioClip1);
                }
            }
        }
    }
}