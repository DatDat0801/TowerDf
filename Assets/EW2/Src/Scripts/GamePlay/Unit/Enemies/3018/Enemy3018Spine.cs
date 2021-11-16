using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3018Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private string attackMeleeName3;

        public Enemy3018Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            attackMeleeName2 = "attack_melee_2";
            attackMeleeName3 = "attack_melee_3";
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_melee":
                    var enemy = (Enemy3018) owner;

                    enemy.normalAttackBox.Trigger();
                    break;
            }
        }

        protected override void CompleteAnimation(TrackEntry trackEntry)
        {
            var animationName = trackEntry.Animation.Name;
            if (animationName == appearName)
            {
                var enemy = (Enemy3018) owner;
                // enemy.SetMoveState();
            }
        }

        public override TrackEntry AttackMelee()
        {
            attackMeleeName = CalculateAttackMeleeAnimation();
            var trackEntry = SetAnimation(0, attackMeleeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        private string CalculateAttackMeleeAnimation()
        {
            var animationRandom = Random.Range(0, 3);
            switch (animationRandom)
            {
                case 0:
                    return attackMeleeName1;
                case 1:
                    return attackMeleeName2;
                default:
                    return attackMeleeName3;
            }
        }
    }
}