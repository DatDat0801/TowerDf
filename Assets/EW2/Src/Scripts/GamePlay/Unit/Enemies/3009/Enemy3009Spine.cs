using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3009Spine : DummySpine
    {
        private string attackMeleeName1;
        private string attackMeleeName2;
        private Enemy3009 enemy;

        public Enemy3009Spine(Unit owner) : base(owner)
        {
            attackMeleeName1 = "attack_melee_1";
            enemy = (Enemy3009) owner;
        }
        
        public override TrackEntry AttackMelee()
        {
            var trackEntry = SetAnimation(0, attackMeleeName1, false);
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
                    break;
            }
        }

       
        
       
    }
}