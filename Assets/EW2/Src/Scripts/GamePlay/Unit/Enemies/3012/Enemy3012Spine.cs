using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3012Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private Enemy3012 enemy3012;

        public Enemy3012Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            attackMeleeName2 = "attack_melee_2";
            enemy3012 = (Enemy3012) owner;
        }

        public override TrackEntry AttackMelee()
        {
            var trackEntry = SetAnimation(0, CalculateAttackMeleeAnimation(), false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
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

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_melee":
                    if (trackEntry.Animation.Name.Equals(attackMeleeName1))
                    {
                        enemy3012.normalAttackBox1.Trigger();
                    }
                    else if (trackEntry.Animation.Name.Equals(attackMeleeName2))
                    {
                        enemy3012.normalAttackBox2.Trigger();
                    }

                    break;
                case "spawn_orc":
                    enemy3012.SpawnEnemies();
                    break;
            }
        }
    }
}