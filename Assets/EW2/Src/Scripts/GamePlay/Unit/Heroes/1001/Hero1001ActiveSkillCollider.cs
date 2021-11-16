
namespace EW2
{
    public class Hero1001ActiveSkillCollider : DamageBox<Hero1001>
    {
        public override void Trigger(float time = 0, float delayTime = 0)
        {
            base.Trigger(0.5f);
        }
        
        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var enemy = (EnemyBase) target;
            
            if (owner.HeroStatBase.searchTarget != MoveType.All && enemy.MoveType != owner.HeroStatBase.searchTarget)
            {
                return null;
            }
            
            Hero1001ActiveSkill activeSkill = (Hero1001ActiveSkill)owner.SkillController.SkillActive;
            
            target.StatusController.AddStatus(activeSkill.GetStun(target));

            var damageInfo = new DamageInfo
            {
                creator = owner,
                
                damageType = owner.DamageType,
                
                value = activeSkill.GetDamage(), target = target
            };


            return damageInfo;
        }
    }
}

