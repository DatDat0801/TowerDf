namespace EW2
{
    public class Hero1001PassiveSkill1Collider : DamageBox<Hero1001>
    {
        private const int SkillId = 0;
        
        private Hero1001PassiveSkill1 passiveSkill1;

        private void Awake()
        {
            passiveSkill1 = (Hero1001PassiveSkill1) owner.SkillController.GetSkillPassive(SkillId);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return true;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var enemy = (EnemyBase) target;
            
            if ((owner.HeroStatBase.searchTarget != MoveType.All) && (enemy.MoveType != owner.HeroStatBase.searchTarget))
            {
                return null;
            }
            
            var damageInfo = new DamageInfo
            {
                creator = owner,
                
                damageType = owner.DamageType,
                
                value = passiveSkill1.GetDamage(), target = target
            };

            return damageInfo;
        }
    }
}