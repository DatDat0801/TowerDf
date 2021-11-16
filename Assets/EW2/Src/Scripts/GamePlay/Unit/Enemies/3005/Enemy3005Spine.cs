using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3005Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private string attackMeleeName3;
        private Enemy3005 enemy;

        public Enemy3005Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            attackMeleeName2 = "attack_melee_2";
            attackMeleeName3 = "attack_melee_3";
            enemy = (Enemy3005) owner;
        }
        
        public override TrackEntry AttackMelee()
        {
            attackMeleeName = enemy.IsPhase1()
                ? CalculateAttackMeleeAnimationPhase1()
                : CalculateAttackMeleeAnimationPhase2();
            var trackEntry = SetAnimation(0, attackMeleeName, false);
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
                    else if (trackEntry.Animation.Name.Equals(attackMeleeName2))
                    {
                        enemy.normalAttackBox2.Trigger();
                    }
                    else if (trackEntry.Animation.Name.Equals(attackMeleeName3))
                    {
                        enemy.normalAttackBox3.Trigger();
                    }

                    break;
            }
        }

        private string CalculateAttackMeleeAnimationPhase1()
        {
            var animationRandom = Random.Range(0, 2);
            switch (animationRandom)
            {
                case 0:
                    return attackMeleeName1;
                default:
                    return attackMeleeName2;
            }
        }
        
        private string CalculateAttackMeleeAnimationPhase2()
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