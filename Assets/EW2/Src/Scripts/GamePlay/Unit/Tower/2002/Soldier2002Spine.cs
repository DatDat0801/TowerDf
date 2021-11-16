using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Soldier2002Spine : DummySpine
    {
        private readonly Soldier2002 soldier;

        public Soldier2002Spine(Unit owner) : base(owner)
        {
            attackRangeName = "attack_range_1";

            passive1Name = "skill_1.1";

            passive2Name = "skill_2.1";
            
            soldier = (Soldier2002) owner;
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_range":
                    soldier.SpawnBullet();
                    break;
                case "skill_1.1":
                    soldier.SpawnBulletPassive1();
                    break;
            }
        }

        public override TrackEntry SkillPassive1()
        {
            return SetAnimation(0, passive1Name, false);
        }
        
        public override TrackEntry SkillPassive2()
        {
            return SetAnimation(0, passive2Name, false);
        }
    }
}