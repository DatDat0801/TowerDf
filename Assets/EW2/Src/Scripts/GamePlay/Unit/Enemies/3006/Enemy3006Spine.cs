using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3006Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private Enemy3006 enemy;

        public Enemy3006Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            attackMeleeName2 = "attack_melee_2";
            enemy = (Enemy3006) owner;
        }
        
        public override TrackEntry AttackMelee()
        {
            var trackEntry = SetAnimation(0, CalculateAttackMeleeAnimation(), false);
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

                    break;
            }
        }

        private string CalculateAttackMeleeAnimation()
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
        
       
    }
}