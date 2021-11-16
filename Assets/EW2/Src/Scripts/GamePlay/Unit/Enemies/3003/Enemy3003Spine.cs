using Spine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3003Spine : DummySpine
    {
        public Enemy3003Spine(Unit owner) : base(owner)
        {
            attackMeleeName = "attack_melee_1";
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            var enemy = (Enemy3003)owner;

            switch (e.Data.Name)
            {
                case "attack_melee":
                    
                    if(trackEntry.Animation.Name.Equals(attackMeleeName))
                    {
                        enemy.normalAttackBox.Trigger();
                    }
                    break;
                
                case "attack_range":
                    if(trackEntry.Animation.Name.Equals(attackRangeName))
                    {
                        enemy.SpawnBullet();
                    }
                    break;
            }
        }
    }
}