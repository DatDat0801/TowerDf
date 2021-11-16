

using Spine;

namespace EW2
{
    public class Enemy3007Spine : DummySpine
    {
        public Enemy3007Spine(Unit owner) : base(owner)
        {
            passive1Name = "skill_1";
        }
        
        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            var enemy = (Enemy3007)owner;

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
                
                case "skill_1":
                    if (trackEntry.Animation.Name.Equals(passive1Name))
                    {
                        enemy.PlaySkill();
                    }
                    break;
            }
        }
        
        protected override void CompleteAnimation(TrackEntry trackEntry)
        {
            var animationName = trackEntry.Animation.Name;
            var enemy = (Enemy3007)owner;

            if (animationName == passive1Name && enemy.IsAlive)
            {
                enemy.UnitState.Set(ActionState.Move);
                enemy.UnitSpine.AnimationState.SetAnimation(0, moveName, true);
            }
        }
    }
}
