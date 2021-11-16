using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3004Spine : DummySpine
    {
        public Enemy3004Spine(Unit owner) : base(owner)
        {
            attackMeleeName = "attack_melee";
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_melee":
                    var enemy = (Enemy3004)owner;
                    
                    if(trackEntry.Animation.Name.Equals(attackMeleeName))
                    {
                        enemy.normalAttackBox.Trigger();
                    }
                    break;
            }
        }
    }
}