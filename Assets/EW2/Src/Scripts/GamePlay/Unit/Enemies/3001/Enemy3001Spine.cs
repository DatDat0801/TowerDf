using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3001Spine : DummySpine
    {
        public Enemy3001Spine(Unit owner) : base(owner)
        {
            attackMeleeName = "attack_melee_1";
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_melee":
                    var enemy = (Enemy3001)owner;
                    
                    if(trackEntry.Animation.Name.Equals(attackMeleeName))
                    {
                        enemy.normalAttackBox.Trigger();
                    }
                    break;
            }
        }
    }
}