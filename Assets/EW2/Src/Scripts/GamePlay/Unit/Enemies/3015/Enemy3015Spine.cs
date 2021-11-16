using Hellmade.Sound;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3015Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private Enemy3015 enemy;
        private const string fxAnimation = "fx";
        public Enemy3015Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            enemy = (Enemy3015) owner;
        }
        
        public override TrackEntry AttackMelee()
        {
            var trackEntry1 =  SetAnimation(1, fxAnimation, true);
            SetTrackEntryTimeScaleBySpeed(ref trackEntry1);
            var trackEntry2 = SetAnimation(0, attackMeleeName1, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry2);
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

        public override TrackEntry Move()
        {
            SetAnimation(1, fxAnimation, true);
            return base.Move();
        }

        public override TrackEntry Die()
        {
            SetAnimation(1, fxAnimation, true);
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3015_DIE);
            EazySoundManager.PlaySound(audioClip);
            return base.Die();
        }

        public override TrackEntry Appear()
        {
            SetAnimation(1, fxAnimation, true);
            return base.Appear();
        }

        public override TrackEntry Stun()
        {
            SetAnimation(1, fxAnimation, true);
            return base.Stun();
        }

        public override TrackEntry Idle()
        {
            SetAnimation(1, fxAnimation, true);
            return base.Idle();
        }

        public override TrackEntry Reborn()
        {
            SetAnimation(1, fxAnimation, true);
            return base.Reborn();
        }

        public override TrackEntry AttackRange()
        {
            var trackEntry1 =  SetAnimation(1, fxAnimation, true);
            SetTrackEntryTimeScaleBySpeed(ref trackEntry1);
            return base.AttackRange();
        }
        
    }
}